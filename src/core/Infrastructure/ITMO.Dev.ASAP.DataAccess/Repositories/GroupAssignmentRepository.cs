using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.DataAccess.Contexts;
using ITMO.Dev.ASAP.DataAccess.Mapping;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Domain.Study.GroupAssignments;
using Microsoft.EntityFrameworkCore;

namespace ITMO.Dev.ASAP.DataAccess.Repositories;

public class GroupAssignmentRepository :
    RepositoryBase<GroupAssignment, GroupAssignmentModel>,
    IGroupAssignmentRepository
{
    private readonly DatabaseContext _context;

    public GroupAssignmentRepository(DatabaseContext context) : base(context)
    {
        _context = context;
    }

    protected override DbSet<GroupAssignmentModel> DbSet => _context.GroupAssignments;

    public IAsyncEnumerable<GroupAssignment> QueryAsync(GroupAssignmentQuery query, CancellationToken cancellationToken)
    {
        IQueryable<GroupAssignmentModel> queryable = DbSet;

        if (query.GroupIds.Count is not 0)
        {
            queryable = queryable.Where(x => query.GroupIds.Contains(x.StudentGroupId));
        }

        if (query.AssignmentIds.Count is not 0)
        {
            queryable = queryable.Where(x => query.AssignmentIds.Contains(x.AssignmentId));
        }

        if (query.SubjectCourseIds.Count is not 0)
        {
            queryable = queryable.Where(x => query.SubjectCourseIds.Contains(x.Assignment.SubjectCourseId));
        }

        var finalQueryable = queryable.Select(groupAssignment => new
        {
            groupAssignment,
            groupName = groupAssignment.StudentGroup.Name,
            assignmentTitle = groupAssignment.Assignment.Title,
            assignmentShortName = groupAssignment.Assignment.ShortName,
        });

        return finalQueryable
            .AsAsyncEnumerable()
            .Select(x => GroupAssignmentMapper.MapTo(
                x.groupAssignment,
                x.groupName,
                x.assignmentTitle,
                x.assignmentShortName));
    }

    protected override GroupAssignmentModel MapFrom(GroupAssignment entity)
    {
        return GroupAssignmentMapper.MapFrom(entity);
    }

    protected override bool Equal(GroupAssignment entity, GroupAssignmentModel model)
    {
        return entity.Id.StudentGroupId.Equals(model.StudentGroupId)
               && entity.Id.AssignmentId.Equals(model.AssignmentId);
    }

    protected override void UpdateModel(GroupAssignmentModel model, GroupAssignment entity)
    {
        model.Deadline = entity.Deadline;
    }
}