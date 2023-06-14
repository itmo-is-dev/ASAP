using ITMO.Dev.ASAP.Domain.ValueObject;

namespace ITMO.Dev.ASAP.Domain.Deadlines.DeadlinePenalties;

public abstract class DeadlinePenalty : IEquatable<DeadlinePenalty>
{
    protected DeadlinePenalty(TimeSpan spanBeforeActivation)
    {
        SpanBeforeActivation = spanBeforeActivation;
    }

    public TimeSpan SpanBeforeActivation { get; protected init; }

    public bool Equals(DeadlinePenalty? other)
    {
        return other?.SpanBeforeActivation.Equals(SpanBeforeActivation) ?? false;
    }

    public abstract Points Apply(Points points);

    public sealed override bool Equals(object? obj)
    {
        return Equals(obj as DeadlinePenalty);
    }

    public override int GetHashCode()
    {
        return SpanBeforeActivation.GetHashCode();
    }

    public override string ToString()
    {
        return $"{GetType()} with span {SpanBeforeActivation}";
    }
}