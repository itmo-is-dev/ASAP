using Bogus;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.DataAccess.Models.Users;
using ITMO.Dev.ASAP.Seeding.Options;

namespace ITMO.Dev.ASAP.Seeding.EntityGenerators;

public class StudentGroupGenerator : EntityGeneratorBase<StudentGroupModel>
{
    private const int MinGroupNumber = 10000;
    private const int MaxGroupNumber = 100000;

    private readonly Faker _faker;

    public StudentGroupGenerator(EntityGeneratorOptions<StudentGroupModel> options, Faker faker)
        : base(options)
    {
        _faker = faker;
    }

    protected override StudentGroupModel Generate(int index)
    {
        int groupNumber = _faker.Random.Int(MinGroupNumber, MaxGroupNumber);

        return new StudentGroupModel(_faker.Random.Guid(), $"M{groupNumber}")
        {
            Students = new List<StudentModel>(),
            SubjectCourseGroups = new List<SubjectCourseGroupModel>(),
        };
    }
}