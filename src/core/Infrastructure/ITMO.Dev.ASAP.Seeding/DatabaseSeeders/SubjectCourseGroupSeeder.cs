using ITMO.Dev.ASAP.DataAccess.Contexts;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Seeding.EntityGenerators;

namespace ITMO.Dev.ASAP.Seeding.DatabaseSeeders;

public class SubjectCourseGroupSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<SubjectCourseGroupModel> _generator;

    public int Priority => 1;

    public SubjectCourseGroupSeeder(IEntityGenerator<SubjectCourseGroupModel> generator)
    {
        _generator = generator;
    }

    public void Seed(DatabaseContext context)
    {
        context.SubjectCourseGroups.AddRange(_generator.GeneratedEntities);
    }
}