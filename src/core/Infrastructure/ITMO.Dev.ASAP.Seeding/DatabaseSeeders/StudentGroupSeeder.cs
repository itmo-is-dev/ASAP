using ITMO.Dev.ASAP.DataAccess.Contexts;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Seeding.EntityGenerators;

namespace ITMO.Dev.ASAP.Seeding.DatabaseSeeders;

public class StudentGroupSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<StudentGroupModel> _generator;

    public StudentGroupSeeder(IEntityGenerator<StudentGroupModel> generator)
    {
        _generator = generator;
    }

    public void Seed(DatabaseContext context)
    {
        context.StudentGroups.AddRange(_generator.GeneratedEntities);
    }
}