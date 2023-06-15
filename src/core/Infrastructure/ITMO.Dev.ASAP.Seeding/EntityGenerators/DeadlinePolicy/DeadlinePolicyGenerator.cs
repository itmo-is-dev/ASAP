using Bogus;
using ITMO.Dev.ASAP.DataAccess.Models.DeadlinePenalties;
using ITMO.Dev.ASAP.Seeding.Extensions;
using ITMO.Dev.ASAP.Seeding.Options;

namespace ITMO.Dev.ASAP.Seeding.EntityGenerators;

public class DeadlinePolicyGenerator : EntityGeneratorBase<DeadlinePenaltyModel>
{
    private readonly Faker _faker;

    public DeadlinePolicyGenerator(EntityGeneratorOptions<DeadlinePenaltyModel> options, Faker faker)
        : base(options)
    {
        _faker = faker;
    }

    protected override DeadlinePenaltyModel Generate(int index)
    {
        return (index % 3) switch
        {
            0 => new AbsoluteDeadlinePenaltyModel(
                _faker.Random.Guid(),
                Guid.Empty,
                _faker.Date.Timespan(),
                _faker.Random.Points(0, 10).Value),

            1 => new FractionDeadlinePenaltyModel(
                _faker.Random.Guid(),
                Guid.Empty,
                _faker.Date.Timespan(),
                _faker.Random.Double()),

            2 => new CappingDeadlinePenaltyModel(
                _faker.Random.Guid(),
                Guid.Empty,
                _faker.Date.Timespan(),
                _faker.Random.Double(0, 5)),

            _ => throw new ArgumentOutOfRangeException(nameof(index)),
        };
    }
}