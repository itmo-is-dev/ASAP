using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.DataAccess.Contexts;
using ITMO.Dev.ASAP.DataAccess.Mapping;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.DataAccess.Models.DeadlinePenalties;
using ITMO.Dev.ASAP.Domain.Deadlines.DeadlinePenalties;
using ITMO.Dev.ASAP.Domain.Groups;
using ITMO.Dev.ASAP.Domain.Study.Assignments;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses.Events;
using ITMO.Dev.ASAP.Domain.Users;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace ITMO.Dev.ASAP.DataAccess.Repositories.SubjectCourses;

public class SubjectCourseRepository : ISubjectCourseRepository
{
    private readonly DatabaseContext _context;
    private readonly ConcurrentDictionary<DeadlinePenalty, DeadlinePenaltyModel> _penaltyCache;
    private readonly ISubjectCourseEventVisitor _visitor;

    public SubjectCourseRepository(DatabaseContext context)
    {
        _context = context;
        _penaltyCache = new ConcurrentDictionary<DeadlinePenalty, DeadlinePenaltyModel>();
        _visitor = new SubjectCourseRepositoryEventVisitor(context);
    }

    public IAsyncEnumerable<SubjectCourse> QueryAsync(SubjectCourseQuery query, CancellationToken cancellationToken)
    {
        IQueryable<SubjectCourseModel> queryable = _context.SubjectCourses
            .Include(x => x.DeadlinePenalties);

        if (query.Ids is not [])
        {
            queryable = queryable.Where(x => query.Ids.Contains(x.Id));
        }

        if (query.SubjectIds is not [])
        {
            queryable = queryable.Where(x => query.SubjectIds.Contains(x.SubjectId));
        }

        if (query.AssignmentIds is not [])
        {
            queryable = queryable.Where(sc => sc.Assignments
                .Select(x => x.Id)
                .Any(x => query.AssignmentIds.Contains(x)));
        }

        if (query.StudentGroupIds is not [])
        {
            queryable = queryable.Where(sc => sc.SubjectCourseGroups
                .Select(x => x.StudentGroupId)
                .Any(x => query.StudentGroupIds.Contains(x)));
        }

        if (query.SubmissionIds is not [])
        {
            queryable = queryable.Where(sc => sc.Assignments
                .SelectMany(x => x.GroupAssignments)
                .SelectMany(x => x.Submissions)
                .Any(x => query.SubmissionIds.Contains(x.Id)));
        }

        var finalQueryable = queryable.Select(sc => new
        {
            subjectCourse = sc,
            groups = sc.SubjectCourseGroups.Select(g => new { g.StudentGroupId, g.StudentGroup.Name }),
            assignments = sc.Assignments.Select(a => new
            {
                id = a.Id,
                title = a.Title,
                shortName = a.ShortName,
                groups = a.GroupAssignments.Select(g => new
                {
                    id = g.StudentGroupId,
                    name = g.StudentGroup.Name,
                }),
            }),
            mentors = sc.Mentors.Select(x => x.UserId),
        });

        return finalQueryable
            .AsSplitQuery()
            .AsAsyncEnumerable()
            .Select(x =>
            {
                var groups = x.groups
                    .Select(g => new StudentGroupInfo(g.StudentGroupId, g.Name))
                    .ToHashSet();

                IEnumerable<SubjectCourseAssignment> assignments = x.assignments.Select(a =>
                {
                    var assignmentGroups = a.groups
                        .Select(g => new StudentGroupInfo(g.id, g.name))
                        .ToHashSet();

                    var assignmentInfo = new AssignmentInfo(a.id, a.title, a.shortName);

                    return new SubjectCourseAssignment(assignmentInfo, assignmentGroups);
                });

                IEnumerable<Mentor> mentors = x.mentors.Select(m => new Mentor(m, x.subjectCourse.Id));

                return MapTo(x.subjectCourse, groups, assignments, mentors);
            });
    }

    public void Add(SubjectCourse subjectCourse)
    {
        SubjectCourseModel model = MapFrom(subjectCourse);
        _context.SubjectCourses.Add(model);
    }

    public ValueTask ApplyAsync(ISubjectCourseEvent evt, CancellationToken cancellationToken)
    {
        return evt.AcceptAsync(_visitor, cancellationToken);
    }

    private SubjectCourse MapTo(
        SubjectCourseModel model,
        IEnumerable<StudentGroupInfo> groups,
        IEnumerable<SubjectCourseAssignment> assignments,
        IEnumerable<Mentor> mentors)
    {
        var penalties = model.DeadlinePenalties
            .Select(MapToDeadlinePenalty)
            .ToHashSet();

        return SubjectCourseMapper.MapTo(
            model,
            groups.ToHashSet(),
            penalties,
            assignments.ToHashSet(),
            mentors.ToHashSet());
    }

    private SubjectCourseModel MapFrom(SubjectCourse entity)
    {
        return SubjectCourseMapper.MapFrom(entity);
    }

    private DeadlinePenalty MapToDeadlinePenalty(DeadlinePenaltyModel model)
    {
        DeadlinePenalty penalty = DeadlinePenaltyMapper.MapTo(model);
        _penaltyCache.TryAdd(penalty, model);

        return penalty;
    }
}