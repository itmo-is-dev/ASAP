using Bogus;
using ITMO.Dev.ASAP.Seeding.Options;
using Subject = ITMO.Dev.ASAP.Domain.Study.Subject;

namespace ITMO.Dev.ASAP.Seeding.EntityGenerators;

public class SubjectGenerator : EntityGeneratorBase<Subject>
{
    private readonly Faker _faker;

    public SubjectGenerator(EntityGeneratorOptions<Subject> options, Faker faker)
        : base(options)
    {
        _faker = faker;
    }

    protected override Subject Generate(int index)
    {
        return new Subject(_faker.Random.Guid(), _faker.Commerce.Product());
    }
}