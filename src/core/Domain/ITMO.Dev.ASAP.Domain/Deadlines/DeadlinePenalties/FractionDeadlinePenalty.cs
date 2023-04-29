using ITMO.Dev.ASAP.Core.ValueObject;

namespace ITMO.Dev.ASAP.Core.Deadlines.DeadlinePenalties;

public class FractionDeadlinePenalty : DeadlinePenalty
{
    public FractionDeadlinePenalty(TimeSpan spanBeforeActivation, Fraction fraction)
        : base(spanBeforeActivation)
    {
        Fraction = fraction;
    }

    protected FractionDeadlinePenalty() { }

    public Fraction Fraction { get; set; }

    public override Points Apply(Points points)
    {
        return points * Fraction;
    }
}