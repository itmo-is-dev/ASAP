using ITMO.Dev.ASAP.Domain.ValueObject;

namespace ITMO.Dev.ASAP.Domain.Deadlines.DeadlinePenalties;

public class FractionDeadlinePenalty : DeadlinePenalty
{
    public FractionDeadlinePenalty(TimeSpan spanBeforeActivation, Fraction fraction)
        : base(spanBeforeActivation)
    {
        Fraction = fraction;
    }

    public Fraction Fraction { get; set; }

    public override Points Apply(Points points)
    {
        return points * Fraction;
    }
}