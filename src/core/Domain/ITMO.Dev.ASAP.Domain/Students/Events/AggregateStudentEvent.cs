namespace ITMO.Dev.ASAP.Domain.Students.Events;

public class AggregateStudentEvent : IStudentEvent
{
    private readonly IReadOnlyCollection<IStudentEvent> _events;

    public AggregateStudentEvent(IReadOnlyCollection<IStudentEvent> events)
    {
        _events = events;
    }

    public static AggregateStudentEvent Build(Func<Builder, Builder> action)
    {
        return action.Invoke(new Builder()).Build();
    }

    public async ValueTask AcceptAsync(IStudentEventVisitor visitor, CancellationToken cancellationToken)
    {
        foreach (IStudentEvent evt in _events)
        {
            await evt.AcceptAsync(visitor, cancellationToken);
        }
    }

    public class Builder
    {
        private readonly List<IStudentEvent> _events;

        public Builder()
        {
            _events = new List<IStudentEvent>();
        }

        public Builder WithEvent(IStudentEvent evt)
        {
            _events.Add(evt);
            return this;
        }

        public Builder WithEvents(IEnumerable<IStudentEvent> events)
        {
            _events.AddRange(events);
            return this;
        }

        public AggregateStudentEvent Build()
        {
            return new AggregateStudentEvent(_events);
        }
    }
}