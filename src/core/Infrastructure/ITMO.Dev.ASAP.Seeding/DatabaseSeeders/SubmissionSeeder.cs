using ITMO.Dev.ASAP.DataAccess.Contexts;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Seeding.EntityGenerators;

namespace ITMO.Dev.ASAP.Seeding.DatabaseSeeders;

public class SubmissionSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<SubmissionModel> _generator;

    public SubmissionSeeder(IEntityGenerator<SubmissionModel> generator)
    {
        _generator = generator;
    }

    public void Seed(DatabaseContext context)
    {
        context.Submissions.AddRange(_generator.GeneratedEntities);
    }
}