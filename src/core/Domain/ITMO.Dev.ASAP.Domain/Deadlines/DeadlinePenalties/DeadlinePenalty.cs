using ITMO.Dev.ASAP.Core.ValueObject;

namespace ITMO.Dev.ASAP.Core.Deadlines.DeadlinePenalty;

public abstract class DeadlinePenalty : IEquatable<DeadlinePenalty>
{
    protected DeadlinePenalty(TimeSpan spanBeforeActivation)
    {
        SpanBeforeActivation = spanBeforeActivation;
    }

    protected DeadlinePenalty() { }

    public TimeSpan SpanBeforeActivation { get; protected init; }

    public virtual bool Equals(DeadlinePenalty? other)
    {
        return other?.SpanBeforeActivation.Equals(SpanBeforeActivation) ?? false;
    }

    public abstract Points Apply(Points points);

    public override bool Equals(object? obj)
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