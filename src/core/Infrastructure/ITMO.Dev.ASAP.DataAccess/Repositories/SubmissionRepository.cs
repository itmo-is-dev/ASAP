using ITMO.Dev.ASAP.Application.DataAccess.Models;
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.DataAccess.Contexts;
using ITMO.Dev.ASAP.DataAccess.Mapping;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.DataAccess.Tools;
using ITMO.Dev.ASAP.Domain.Submissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ITMO.Dev.ASAP.DataAccess.Repositories;

public class SubmissionRepository : ISubmissionRepository
{
    private readonly DatabaseContext _context;

    public SubmissionRepository(DatabaseContext context)
    {
        _context = context;
    }

    public IAsyncEnumerable<Submission> QueryAsync(SubmissionQuery query, CancellationToken cancellationToken)
    {
        IQueryable<SubmissionModel> queryable = ApplyQuery(_context.Submissions, query);

        queryable = queryable
            .Include(x => x.Student)
            .ThenInclude(x => x.User)
            .ThenInclude(x => x.Associations);

        var finalQueryable = queryable.Select(submission => new
        {
            submission,
            groupAssignment = submission.GroupAssignment,
            groupName = submission.GroupAssignment.StudentGroup.Name,
            assignmentTitle = submission.GroupAssignment.Assignment.Title,
            assignmentShortName = submission.GroupAssignment.Assignment.ShortName,
        });

        return finalQueryable
            .AsAsyncEnumerable()
            .Select(x => SubmissionMapper.MapTo(
                x.submission,
                GroupAssignmentMapper.MapTo(x.groupAssignment, x.groupName, x.assignmentTitle, x.assignmentShortName)));
    }

    public Task<int> CountAsync(SubmissionQuery query, CancellationToken cancellationToken)
    {
        IQueryable<SubmissionModel> queryable = ApplyQuery(_context.Submissions, query);
        return queryable.CountAsync(cancellationToken);
    }

    public void Add(Submission submission)
    {
        SubmissionModel model = SubmissionMapper.MapFrom(submission);
        _context.Submissions.Add(model);
    }

    public void Update(Submission submission)
    {
        EntityEntry<SubmissionModel> entry = RepositoryTools.GetEntry(
            _context,
            x => x.Id.Equals(submission.Id),
            () => SubmissionMapper.MapFrom(submission));

        SubmissionModel model = entry.Entity;
        model.Rating = submission.Rating?.Value;
        model.ExtraPoints = submission.ExtraPoints?.Value;
        model.SubmissionDate = submission.SubmissionDate;
        model.State = submission.State.Kind;

        entry.State = EntityState.Modified;
    }

    private IQueryable<SubmissionModel> ApplyQuery(IQueryable<SubmissionModel> queryable, SubmissionQuery query)
    {
        if (query.Ids is not [])
        {
            queryable = queryable.Where(x => query.Ids.Contains(x.Id));
        }

        if (query.Codes is not [])
        {
            queryable = queryable.Where(x => query.Codes.Contains(x.Code));
        }

        if (query.UserIds is not [])
        {
            queryable = queryable.Where(x => query.UserIds.Contains(x.StudentId));
        }

        if (query.SubjectCourseIds is not [])
        {
            queryable = queryable
                .Where(x => query.SubjectCourseIds.Contains(x.GroupAssignment.Assignment.SubjectCourseId));
        }

        if (query.AssignmentIds is not [])
        {
            queryable = queryable.Where(x => query.AssignmentIds.Contains(x.AssignmentId));
        }

        if (query.StudentGroupIds is not [])
        {
            queryable = queryable.Where(x => query.StudentGroupIds.Contains(x.StudentGroupId));
        }

        if (query.SubmissionStates is not [])
        {
            queryable = queryable.Where(x => query.SubmissionStates.Contains(x.State));
        }

        if (query.SubjectCourseWorkflows is not [])
        {
            queryable = queryable
                .Where(x => query.SubjectCourseWorkflows
                    .Contains(x.GroupAssignment.Assignment.SubjectCourse.WorkflowType!.Value));
        }

        if (query.Limit is not null)
        {
            queryable = queryable.Take(query.Limit.Value);
        }

        if (query.OrderByCode is not null)
        {
            queryable = query.OrderByCode.Value switch
            {
                OrderDirection.Ascending => queryable.OrderBy(x => x.Code),
                OrderDirection.Descending => queryable.OrderByDescending(x => x.Code),
                _ => throw new ArgumentOutOfRangeException(nameof(query)),
            };
        }

        return queryable;
    }
}