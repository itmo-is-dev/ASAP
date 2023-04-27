using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Seeding.EntityGenerators;
using SubjectCourse = ITMO.Dev.ASAP.Domain.Study.SubjectCourse;

namespace ITMO.Dev.ASAP.Seeding.DatabaseSeeders;

public class SubjectCourseSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<SubjectCourse> _generator;

    public SubjectCourseSeeder(IEntityGenerator<SubjectCourse> generator)
    {
        _generator = generator;
    }

    public void Seed(IDatabaseContext context)
    {
        context.SubjectCourses.AddRange(_generator.GeneratedEntities);
    }
}