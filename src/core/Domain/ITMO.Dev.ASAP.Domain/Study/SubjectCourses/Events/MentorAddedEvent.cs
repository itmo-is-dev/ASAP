using ITMO.Dev.ASAP.Domain.Users;

namespace ITMO.Dev.ASAP.Domain.Study.SubjectCourses.Events;

public class MentorAddedEvent : ISubjectCourseEvent
{
    public MentorAddedEvent(Mentor mentor)
    {
        Mentor = mentor;
    }

    public Mentor Mentor { get; }

    public ValueTask AcceptAsync(ISubjectCourseEventVisitor visitor, CancellationToken cancellationToken)
    {
        return visitor.VisitAsync(this, cancellationToken);
    }
}