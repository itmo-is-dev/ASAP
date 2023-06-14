using Bogus;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Seeding.Options;

namespace ITMO.Dev.ASAP.Seeding.EntityGenerators;

public class SubjectGenerator : EntityGeneratorBase<SubjectModel>
{
    private readonly Faker _faker;

    public SubjectGenerator(EntityGeneratorOptions<SubjectModel> options, Faker faker)
        : base(options)
    {
        _faker = faker;
    }

    protected override SubjectModel Generate(int index)
    {
        return new SubjectModel(_faker.Random.Guid(), _faker.Commerce.Product())
        {
            SubjectCourses = new List<SubjectCourseModel>(),
        };
    }
}