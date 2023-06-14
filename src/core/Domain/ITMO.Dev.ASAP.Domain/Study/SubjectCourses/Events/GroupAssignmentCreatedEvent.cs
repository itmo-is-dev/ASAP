using ITMO.Dev.ASAP.Domain.Study.GroupAssignments;

namespace ITMO.Dev.ASAP.Domain.Study.SubjectCourses.Events;

public class GroupAssignmentCreatedEvent : ISubjectCourseEvent
{
    public GroupAssignmentCreatedEvent(GroupAssignment groupAssignment)
    {
        GroupAssignment = groupAssignment;
    }

    public GroupAssignment GroupAssignment { get; }

    public ValueTask AcceptAsync(ISubjectCourseEventVisitor visitor, CancellationToken cancellationToken)
    {
        return visitor.VisitAsync(this, cancellationToken);
    }
}