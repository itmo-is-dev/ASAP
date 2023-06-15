namespace ITMO.Dev.ASAP.Domain.Study.SubjectCourses.Events;

public interface ISubjectCourseEvent
{
    ValueTask AcceptAsync(ISubjectCourseEventVisitor visitor, CancellationToken cancellationToken);
}