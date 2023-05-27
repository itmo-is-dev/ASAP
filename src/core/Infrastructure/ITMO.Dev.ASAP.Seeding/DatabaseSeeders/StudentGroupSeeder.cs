using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Seeding.EntityGenerators;
using StudentGroup = ITMO.Dev.ASAP.Domain.Study.StudentGroup;

namespace ITMO.Dev.ASAP.Seeding.DatabaseSeeders;

public class StudentGroupSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<StudentGroup> _generator;

    public StudentGroupSeeder(IEntityGenerator<StudentGroup> generator)
    {
        _generator = generator;
    }

    public void Seed(IDatabaseContext context)
    {
        context.StudentGroups.AddRange(_generator.GeneratedEntities);
    }
}