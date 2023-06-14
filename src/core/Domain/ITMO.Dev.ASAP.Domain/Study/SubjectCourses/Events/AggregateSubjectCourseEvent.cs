namespace ITMO.Dev.ASAP.Domain.Study.SubjectCourses.Events;

public class AggregateSubjectCourseEvent : ISubjectCourseEvent
{
    private readonly IReadOnlyCollection<ISubjectCourseEvent> _events;

    private AggregateSubjectCourseEvent(IReadOnlyCollection<ISubjectCourseEvent> events)
    {
        _events = events;
    }

    public static AggregateSubjectCourseEvent Build(Func<Builder, Builder> action)
    {
        return action.Invoke(new Builder()).Build();
    }

    public async ValueTask AcceptAsync(ISubjectCourseEventVisitor visitor, CancellationToken cancellationToken)
    {
        foreach (ISubjectCourseEvent evt in _events)
        {
            await evt.AcceptAsync(visitor, cancellationToken);
        }
    }

    public class Builder
    {
        private readonly List<ISubjectCourseEvent> _events;

        public Builder()
        {
            _events = new List<ISubjectCourseEvent>();
        }

        public Builder WithEvent(ISubjectCourseEvent evt)
        {
            _events.Add(evt);
            return this;
        }

        public Builder WithEvents(IEnumerable<ISubjectCourseEvent> events)
        {
            _events.AddRange(events);
            return this;
        }

        public AggregateSubjectCourseEvent Build()
        {
            return new AggregateSubjectCourseEvent(_events);
        }
    }
}