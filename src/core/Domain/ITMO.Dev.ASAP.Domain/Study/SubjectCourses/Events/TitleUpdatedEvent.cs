namespace ITMO.Dev.ASAP.Domain.Study.SubjectCourses.Events;

public class TitleUpdatedEvent : ISubjectCourseEvent
{
    public TitleUpdatedEvent(SubjectCourse subjectCourse, string title)
    {
        SubjectCourse = subjectCourse;
        Title = title;
    }

    public SubjectCourse SubjectCourse { get; }

    public string Title { get; }

    public ValueTask AcceptAsync(ISubjectCourseEventVisitor visitor, CancellationToken cancellationToken)
    {
        return visitor.VisitAsync(this, cancellationToken);
    }
}