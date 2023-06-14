using ITMO.Dev.ASAP.DataAccess.Contexts;
using ITMO.Dev.ASAP.DataAccess.Models.Users;
using ITMO.Dev.ASAP.Seeding.EntityGenerators;

namespace ITMO.Dev.ASAP.Seeding.DatabaseSeeders;

public class StudentSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<StudentModel> _generator;

    public StudentSeeder(IEntityGenerator<StudentModel> generator)
    {
        _generator = generator;
    }

    public int Priority => 1;

    public void Seed(DatabaseContext context)
    {
        context.Students.AddRange(_generator.GeneratedEntities);
    }
}