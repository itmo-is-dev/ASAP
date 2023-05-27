using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Seeding.EntityGenerators;
using Subject = ITMO.Dev.ASAP.Domain.Study.Subject;

namespace ITMO.Dev.ASAP.Seeding.DatabaseSeeders;

public class SubjectSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<Subject> _generator;

    public SubjectSeeder(IEntityGenerator<Subject> generator)
    {
        _generator = generator;
    }

    public void Seed(IDatabaseContext context)
    {
        context.Subjects.AddRange(_generator.GeneratedEntities);
    }
}