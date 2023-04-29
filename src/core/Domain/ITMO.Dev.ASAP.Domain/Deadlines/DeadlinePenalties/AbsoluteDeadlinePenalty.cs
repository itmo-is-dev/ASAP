using ITMO.Dev.ASAP.Core.ValueObject;

namespace ITMO.Dev.ASAP.Core.Deadlines.DeadlinePenalties;

public class AbsoluteDeadlinePenalty : DeadlinePenalty
{
    public AbsoluteDeadlinePenalty(TimeSpan spanBeforeActivation, Points absoluteValue)
        : base(spanBeforeActivation)
    {
        AbsoluteValue = absoluteValue;
    }

    protected AbsoluteDeadlinePenalty() { }

    public Points AbsoluteValue { get; set; }

    public override Points Apply(Points points)
    {
        return points - AbsoluteValue;
    }
}