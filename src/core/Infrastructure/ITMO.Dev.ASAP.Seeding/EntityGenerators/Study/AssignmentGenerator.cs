using Bogus;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Seeding.Extensions;
using ITMO.Dev.ASAP.Seeding.Options;

namespace ITMO.Dev.ASAP.Seeding.EntityGenerators;

public class AssignmentGenerator : EntityGeneratorBase<AssignmentModel>
{
    private readonly Faker _faker;
    private readonly IEntityGenerator<SubjectCourseModel> _subjectCourseGenerator;

    public AssignmentGenerator(
        EntityGeneratorOptions<AssignmentModel> options,
        IEntityGenerator<SubjectCourseModel> subjectCourseGenerator,
        Faker faker)
        : base(options)
    {
        _subjectCourseGenerator = subjectCourseGenerator;
        _faker = faker;
    }

    protected override AssignmentModel Generate(int index)
    {
        SubjectCourseModel? subjectCourse = _faker
            .PickRandom<SubjectCourseModel>(_subjectCourseGenerator.GeneratedEntities);

        int order = index + 1;

        Guid id = _faker.Random.Guid();

        var assignment = new AssignmentModel(
            id,
            subjectCourse.Id,
            _faker.Commerce.Product(),
            $"lab-{order}",
            order,
            _faker.Random.Points(0, 5).Value,
            _faker.Random.Points(5, 10).Value);

        assignment.GroupAssignments = subjectCourse.SubjectCourseGroups
            .Select(x => new GroupAssignmentModel(x.StudentGroupId, id, _faker.Date.FutureDateOnly())
            {
                Submissions = new List<SubmissionModel>(),
                Assignment = assignment,
                StudentGroup = x.StudentGroup,
            })
            .ToList();

        return assignment;
    }
}