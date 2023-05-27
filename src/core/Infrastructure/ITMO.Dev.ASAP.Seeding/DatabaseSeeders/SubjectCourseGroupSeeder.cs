using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Seeding.EntityGenerators;
using SubjectCourseGroup = ITMO.Dev.ASAP.Domain.Study.SubjectCourseGroup;

namespace ITMO.Dev.ASAP.Seeding.DatabaseSeeders;

public class SubjectCourseGroupSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<SubjectCourseGroup> _generator;

    public int Priority => 1;

    public SubjectCourseGroupSeeder(IEntityGenerator<SubjectCourseGroup> generator)
    {
        _generator = generator;
    }

    public void Seed(IDatabaseContext context)
    {
        context.SubjectCourseGroups.AddRange(_generator.GeneratedEntities);
    }
}