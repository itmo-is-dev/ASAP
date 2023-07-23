namespace ITMO.Dev.ASAP.Domain.Study.SubjectCourses.Events;

public interface ISubjectCourseEventVisitor
{
    ValueTask VisitAsync(SubjectCourseGroupCreatedEvent evt, CancellationToken cancellationToken);

    ValueTask VisitAsync(GroupAssignmentCreatedEvent evt, CancellationToken cancellationToken);

    ValueTask VisitAsync(SubjectCourseGroupRemovedEvent evt, CancellationToken cancellationToken);

    ValueTask VisitAsync(SubjectCourseAssignmentCreatedEvent evt, CancellationToken cancellationToken);

    ValueTask VisitAsync(DeadlinePenaltyAddedEvent evt, CancellationToken cancellationToken);

    ValueTask VisitAsync(MentorAddedEvent evt, CancellationToken cancellationToken);

    ValueTask VisitAsync(MentorRemovedEvent evt, CancellationToken cancellationToken);

    ValueTask VisitAsync(TitleUpdatedEvent evt, CancellationToken cancellationToken);
}