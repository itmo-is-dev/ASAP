using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.Deadlines.DeadlinePenalties;
using ITMO.Dev.ASAP.Domain.Deadlines.DeadlinePolicies;
using ITMO.Dev.ASAP.Domain.ValueObject;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Deadlines;

public class DeadlinePolicyTest : TestBase
{
    [Fact]
    public void AddEqualPenaltiesShouldThrow()
    {
        FractionDeadlinePenalty fractionPenalty = new(TimeSpan.FromDays(7), Fraction.FromDenormalizedValue(20));
        AbsoluteDeadlinePenalty absolutePenalty = new(TimeSpan.FromDays(7), new Points(1));

        DeadlinePolicy policy = new(Guid.NewGuid());
        policy.AddDeadlinePenalty(fractionPenalty);

        Assert.Throws<DomainInvalidOperationException>(() => policy.AddDeadlinePenalty(absolutePenalty));
    }

    [Fact]
    public void PointPenaltyTest()
    {
        var fraction = Fraction.FromDenormalizedValue(20);
        FractionDeadlinePenalty fractionPenalty = new(TimeSpan.FromDays(7), fraction);
        var absolutePoints = new Points(1);
        AbsoluteDeadlinePenalty absolutePenalty = new(TimeSpan.FromDays(14), absolutePoints);

        DeadlinePolicy policy = new(Guid.NewGuid());
        policy.AddDeadlinePenalty(fractionPenalty);
        policy.AddDeadlinePenalty(absolutePenalty);

        Points maxPoints = new(10);
        Points? penaltyPoints1 = policy.GetPointPenalty(
            maxPoints,
            new DateOnly(2000, 10, 10),
            new DateOnly(2000, 10, 18));

        Assert.Equal(penaltyPoints1, maxPoints * fraction.Value);

        Points? penaltyPoints2 = policy.GetPointPenalty(
            maxPoints,
            new DateOnly(2000, 10, 10),
            new DateOnly(2003, 1, 1));

        Assert.Equal(penaltyPoints2, maxPoints - absolutePoints);
    }
}