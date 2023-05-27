using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Seeding.EntityGenerators;

namespace ITMO.Dev.ASAP.Seeding.DatabaseSeeders;

public class AssignmentDatabaseSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<Assignment> _generator;

    public AssignmentDatabaseSeeder(IEntityGenerator<Assignment> generator)
    {
        _generator = generator;
    }

    public void Seed(IDatabaseContext context)
    {
        context.Assignments.AddRange(_generator.GeneratedEntities);
    }
}