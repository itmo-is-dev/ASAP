namespace ITMO.Dev.ASAP.Domain.Study.SubjectCourses.Events;

public class SubjectCourseGroupCreatedEvent : ISubjectCourseEvent
{
    public SubjectCourseGroupCreatedEvent(SubjectCourseGroup subjectCourseGroup)
    {
        SubjectCourseGroup = subjectCourseGroup;
    }

    public SubjectCourseGroup SubjectCourseGroup { get; }

    public ValueTask AcceptAsync(ISubjectCourseEventVisitor visitor, CancellationToken cancellationToken)
    {
        return visitor.VisitAsync(this, cancellationToken);
    }
}