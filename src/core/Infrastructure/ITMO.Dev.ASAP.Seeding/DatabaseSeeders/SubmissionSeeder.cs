using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Seeding.EntityGenerators;

namespace ITMO.Dev.ASAP.Seeding.DatabaseSeeders;

public class SubmissionSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<Submission> _generator;

    public SubmissionSeeder(IEntityGenerator<Submission> generator)
    {
        _generator = generator;
    }

    public void Seed(IDatabaseContext context)
    {
        context.Submissions.AddRange(_generator.GeneratedEntities);
    }
}