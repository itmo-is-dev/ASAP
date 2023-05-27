using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Seeding.EntityGenerators;
using Student = ITMO.Dev.ASAP.Domain.Users.Student;

namespace ITMO.Dev.ASAP.Seeding.DatabaseSeeders;

public class StudentSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<Student> _generator;

    public StudentSeeder(IEntityGenerator<Student> generator)
    {
        _generator = generator;
    }

    public int Priority => 1;

    public void Seed(IDatabaseContext context)
    {
        context.Students.AddRange(_generator.GeneratedEntities);
    }
}