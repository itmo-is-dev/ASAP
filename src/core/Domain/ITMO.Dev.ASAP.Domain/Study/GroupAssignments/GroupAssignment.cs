using ITMO.Dev.ASAP.Domain.Groups;
using ITMO.Dev.ASAP.Domain.Study.Assignments;
using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Domain.Study.GroupAssignments;

public partial class GroupAssignment : IEntity
{
    public GroupAssignment(
        GroupAssignmentId id,
        DateOnly deadline,
        StudentGroupInfo group,
        AssignmentInfo assignment) : this(id)
    {
        Deadline = deadline;
        Group = group;
        Assignment = assignment;
    }

    [KeyProperty]
    public GroupAssignmentId Id { get; }

    public StudentGroupInfo Group { get; }

    public AssignmentInfo Assignment { get; }

    public DateOnly Deadline { get; set; }

    public override string ToString()
    {
        return $"Assignment: {Id.AssignmentId}, Group: {Id.StudentGroupId}";
    }
}