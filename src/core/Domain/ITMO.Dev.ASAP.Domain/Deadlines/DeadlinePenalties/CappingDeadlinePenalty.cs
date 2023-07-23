using ITMO.Dev.ASAP.Domain.ValueObject;

namespace ITMO.Dev.ASAP.Domain.Deadlines.DeadlinePenalties;

public class CappingDeadlinePenalty : DeadlinePenalty
{
    public CappingDeadlinePenalty(TimeSpan spanBeforeActivation, Points cap)
        : base(spanBeforeActivation)
    {
        Cap = cap;
    }

    public Points Cap { get; set; }

    public override Points Apply(Points points)
    {
        return Points.Min(points, Cap);
    }
}