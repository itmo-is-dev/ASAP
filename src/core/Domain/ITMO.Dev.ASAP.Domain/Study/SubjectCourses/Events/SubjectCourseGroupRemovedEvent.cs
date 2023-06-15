namespace ITMO.Dev.ASAP.Domain.Study.SubjectCourses.Events;

public class SubjectCourseGroupRemovedEvent : ISubjectCourseEvent
{
    public SubjectCourseGroupRemovedEvent(SubjectCourse subjectCourse, Guid studentGroupId)
    {
        SubjectCourse = subjectCourse;
        StudentGroupId = studentGroupId;
    }

    public SubjectCourse SubjectCourse { get; }

    public Guid StudentGroupId { get; }

    public ValueTask AcceptAsync(ISubjectCourseEventVisitor visitor, CancellationToken cancellationToken)
    {
        return visitor.VisitAsync(this, cancellationToken);
    }
}