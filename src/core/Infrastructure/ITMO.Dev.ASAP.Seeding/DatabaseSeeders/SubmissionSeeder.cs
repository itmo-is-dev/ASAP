using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Seeding.EntityGenerators;
using Submission = ITMO.Dev.ASAP.Domain.Submissions.Submission;

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