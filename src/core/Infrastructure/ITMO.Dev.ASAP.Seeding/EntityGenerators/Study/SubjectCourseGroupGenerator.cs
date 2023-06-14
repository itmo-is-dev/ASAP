using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Seeding.Options;

namespace ITMO.Dev.ASAP.Seeding.EntityGenerators;

public class SubjectCourseGroupGenerator : EntityGeneratorBase<SubjectCourseGroupModel>
{
    private readonly IEntityGenerator<StudentGroupModel> _studentGroupGenerator;
    private readonly IEntityGenerator<SubjectCourseModel> _subjectCourseGenerator;

    public SubjectCourseGroupGenerator(
        EntityGeneratorOptions<SubjectCourseGroupModel> options,
        IEntityGenerator<SubjectCourseModel> subjectCourseGenerator,
        IEntityGenerator<StudentGroupModel> studentGroupGenerator)
        : base(options)
    {
        _subjectCourseGenerator = subjectCourseGenerator;
        _studentGroupGenerator = studentGroupGenerator;
    }

    protected override SubjectCourseGroupModel Generate(int index)
    {
        int studentGroupCount = _studentGroupGenerator.GeneratedEntities.Count;
        int studentGroupNumber = index % studentGroupCount;

        int subjectCourseCount = _subjectCourseGenerator.GeneratedEntities.Count;
        int subjectCourseGroupNumber = index / studentGroupCount;

        if (subjectCourseGroupNumber >= subjectCourseCount)
        {
            const string message = "The subject course group index is greater than the number of subject courses.";
            throw new ArgumentOutOfRangeException(nameof(index), message);
        }

        SubjectCourseModel subjectCourse = _subjectCourseGenerator.GeneratedEntities[subjectCourseGroupNumber];
        StudentGroupModel studentGroup = _studentGroupGenerator.GeneratedEntities[studentGroupNumber];

        var subjectCourseGroup = new SubjectCourseGroupModel(subjectCourse.Id, studentGroup.Id);
        subjectCourse.SubjectCourseGroups.Add(subjectCourseGroup);

        return subjectCourseGroup;
    }
}