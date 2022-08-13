using Kysect.Shreks.Core.ValueObject;

namespace Kysect.Shreks.Core.DeadlinePolicies;

public class CappingDeadlinePolicy : DeadlinePolicy
{
    public CappingDeadlinePolicy(TimeSpan spanBeforeActivation, double cap) : base(spanBeforeActivation)
    {
        Cap = cap;
    }

    protected CappingDeadlinePolicy() { }

    public double Cap { get; set; }

    public override Points Apply(Points points)
        => Math.Max(points, Cap);

    public override bool Equals(DeadlinePolicy? other)
    {
        return other is CappingDeadlinePolicy cappingDeadlineSpan &&
               cappingDeadlineSpan.Cap.Equals(Cap) &&
               base.Equals(cappingDeadlineSpan);
    }
}