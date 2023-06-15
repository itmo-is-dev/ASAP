using ITMO.Dev.ASAP.DataAccess.Contexts;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Seeding.EntityGenerators;

namespace ITMO.Dev.ASAP.Seeding.DatabaseSeeders;

public class SubjectCourseSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<SubjectCourseModel> _generator;

    public SubjectCourseSeeder(IEntityGenerator<SubjectCourseModel> generator)
    {
        _generator = generator;
    }

    public void Seed(DatabaseContext context)
    {
        context.SubjectCourses.AddRange(_generator.GeneratedEntities);
    }
}