using Bogus;
using ITMO.Dev.ASAP.Core.Deadlines.DeadlinePenalties;
using ITMO.Dev.ASAP.Seeding.Extensions;
using ITMO.Dev.ASAP.Seeding.Options;

namespace ITMO.Dev.ASAP.Seeding.EntityGenerators;

public class DeadlinePolicyGenerator : EntityGeneratorBase<DeadlinePenalty>
{
    private readonly Faker _faker;

    public DeadlinePolicyGenerator(EntityGeneratorOptions<DeadlinePenalty> options, Faker faker)
        : base(options)
    {
        _faker = faker;
    }

    protected override DeadlinePenalty Generate(int index)
    {
        return (index % 3) switch
        {
            0 => new AbsoluteDeadlinePenalty(_faker.Date.Timespan(), _faker.Random.Points(0, 10)),
            1 => new FractionDeadlinePenalty(_faker.Date.Timespan(), _faker.Random.Double()),
            2 => new CappingDeadlinePenalty(_faker.Date.Timespan(), _faker.Random.Double(0, 5)),
            _ => throw new ArgumentOutOfRangeException(nameof(index)),
        };
    }
}