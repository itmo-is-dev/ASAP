using ITMO.Dev.ASAP.Core.ValueObject;

namespace ITMO.Dev.ASAP.Core.Deadlines.DeadlinePenalty;

public class CappingDeadlinePenalty : DeadlinePenalty
{
    public CappingDeadlinePenalty(TimeSpan spanBeforeActivation, double cap)
        : base(spanBeforeActivation)
    {
        Cap = new Points(cap);
    }

    protected CappingDeadlinePenalty() { }

    public Points Cap { get; set; }

    public override Points Apply(Points points)
    {
        return Points.Min(points, Cap);
    }

    public override bool Equals(DeadlinePenalty? other)
    {
        return other is CappingDeadlinePenalty cappingDeadlineSpan &&
               cappingDeadlineSpan.Cap.Equals(Cap) &&
               base.Equals(cappingDeadlineSpan);
    }
}