using ITMO.Dev.ASAP.Domain.ValueObject;

namespace ITMO.Dev.ASAP.Domain.DeadlinePolicies;

public class FractionDeadlinePolicy : DeadlinePolicy
{
    public FractionDeadlinePolicy(TimeSpan spanBeforeActivation, Fraction fraction)
        : base(spanBeforeActivation)
    {
        Fraction = fraction;
    }

    protected FractionDeadlinePolicy() { }

    public Fraction Fraction { get; set; }

    public override Points Apply(Points points)
    {
        return points * Fraction;
    }

    public override bool Equals(DeadlinePolicy? other)
    {
        return other is FractionDeadlinePolicy fractionDeadlineSpan &&
               fractionDeadlineSpan.Fraction.Equals(Fraction) &&
               base.Equals(fractionDeadlineSpan);
    }
}