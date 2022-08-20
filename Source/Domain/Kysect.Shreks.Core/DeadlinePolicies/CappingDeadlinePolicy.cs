using Kysect.Shreks.Core.ValueObject;

namespace Kysect.Shreks.Core.DeadlinePolicies;

public class CappingDeadlinePolicy : DeadlinePolicy
{
    public CappingDeadlinePolicy(TimeSpan spanBeforeActivation, double cap) : base(spanBeforeActivation)
    {
        Cap = new Points(cap);
    }

    protected CappingDeadlinePolicy() { }

    public Points Cap { get; set; }

    public override Points Apply(Points points)
        => Points.Max(points, Cap);

    public override bool Equals(DeadlinePolicy? other)
    {
        return other is CappingDeadlinePolicy cappingDeadlineSpan &&
               cappingDeadlineSpan.Cap.Equals(Cap) &&
               base.Equals(cappingDeadlineSpan);
    }
}