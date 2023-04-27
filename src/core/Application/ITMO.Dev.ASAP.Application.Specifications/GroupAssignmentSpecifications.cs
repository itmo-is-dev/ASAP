using ITMO.Dev.ASAP.Domain.Study;

namespace ITMO.Dev.ASAP.Application.Specifications;

public static class GroupAssignmentSpecifications
{
    public static IQueryable<GroupAssignment> WithAssignment(this IQueryable<GroupAssignment> queryable, Guid assignmentId)
        => queryable.Where(x => x.AssignmentId.Equals(assignmentId));

    public static IQueryable<GroupAssignment> ForStudent(this IQueryable<GroupAssignment> queryable, Guid studentId)
    {
        return queryable
            .Where(groupAssignment => groupAssignment.Group.Students.Any(student => student.UserId.Equals(studentId)));
    }
}