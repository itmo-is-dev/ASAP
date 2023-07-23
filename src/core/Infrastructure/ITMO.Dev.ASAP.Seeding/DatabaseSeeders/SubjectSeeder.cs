using ITMO.Dev.ASAP.DataAccess.Contexts;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Seeding.EntityGenerators;

namespace ITMO.Dev.ASAP.Seeding.DatabaseSeeders;

public class SubjectSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<SubjectModel> _generator;

    public SubjectSeeder(IEntityGenerator<SubjectModel> generator)
    {
        _generator = generator;
    }

    public void Seed(DatabaseContext context)
    {
        context.Subjects.AddRange(_generator.GeneratedEntities);
    }
}