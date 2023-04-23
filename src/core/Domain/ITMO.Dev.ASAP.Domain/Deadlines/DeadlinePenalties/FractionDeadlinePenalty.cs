using ITMO.Dev.ASAP.Core.ValueObject;

namespace ITMO.Dev.ASAP.Core.Deadlines.DeadlinePenalty;

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

    public override bool Equals(DeadlinePenalty? other)
    {
        return other is FractionDeadlinePenalty fractionDeadlineSpan &&
               fractionDeadlineSpan.Fraction.Equals(Fraction) &&
               base.Equals(fractionDeadlineSpan);
    }
}