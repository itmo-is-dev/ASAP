using Bogus;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.DataAccess.Models.DeadlinePenalties;
using ITMO.Dev.ASAP.DataAccess.Models.Users;
using ITMO.Dev.ASAP.Domain.SubmissionStateWorkflows;
using ITMO.Dev.ASAP.Seeding.Options;

namespace ITMO.Dev.ASAP.Seeding.EntityGenerators;

public class SubjectCourseGenerator : EntityGeneratorBase<SubjectCourseModel>
{
    private readonly IEntityGenerator<DeadlinePenaltyModel> _deadlinePolicyGenerator;
    private readonly Faker _faker;
    private readonly IEntityGenerator<SubjectModel> _subjectGenerator;
    private readonly IEntityGenerator<UserModel> _userGenerator;

    public SubjectCourseGenerator(
        EntityGeneratorOptions<SubjectCourseModel> options,
        IEntityGenerator<UserModel> userGenerator,
        IEntityGenerator<SubjectModel> subjectGenerator,
        Faker faker,
        IEntityGenerator<DeadlinePenaltyModel> deadlinePolicyGenerator)
        : base(options)
    {
        _userGenerator = userGenerator;
        _subjectGenerator = subjectGenerator;
        _faker = faker;
        _deadlinePolicyGenerator = deadlinePolicyGenerator;
    }

    protected override SubjectCourseModel Generate(int index)
    {
        int subjectCount = _subjectGenerator.GeneratedEntities.Count;

        int deadlineCount = _faker.Random.Int(0, _deadlinePolicyGenerator.GeneratedEntities.Count);

        IEnumerable<DeadlinePenaltyModel> deadlines = Enumerable.Range(0, deadlineCount)
            .Select(_ => _faker.Random.Int(0, _deadlinePolicyGenerator.GeneratedEntities.Count - 1))
            .Select(i => _deadlinePolicyGenerator.GeneratedEntities[i])
            .Distinct();

        if (index >= subjectCount)
            throw new IndexOutOfRangeException("The subject index is greater than the number of subjects.");

        SubjectModel subject = _subjectGenerator.GeneratedEntities[index];

        string? subjectCourseName = _faker.Commerce.ProductName();

        const SubmissionStateWorkflowType reviewType = SubmissionStateWorkflowType.ReviewWithDefense;

        var subjectCourse = new SubjectCourseModel(_faker.Random.Guid(), subject.Id, subjectCourseName, reviewType)
        {
            Assignments = new List<AssignmentModel>(),
            DeadlinePenalties = new List<DeadlinePenaltyModel>(),
            Mentors = new List<MentorModel>(),
            SubjectCourseGroups = new List<SubjectCourseGroupModel>(),
        };

        subject.SubjectCourses.Add(subjectCourse);

        IEnumerable<UserModel> users = _faker.Random
            .ListItems(_userGenerator.GeneratedEntities.ToList(), 2)
            .Distinct();

        foreach (UserModel user in users)
        {
            var mentor = new MentorModel(user.Id, subjectCourse.Id)
            {
                User = user,
                SubjectCourse = subjectCourse,
            };

            subjectCourse.Mentors.Add(mentor);
        }

        foreach (DeadlinePenaltyModel deadline in deadlines)
        {
            subjectCourse.DeadlinePenalties.Add(deadline);
            deadline.SubjectCourseId = subjectCourse.Id;
        }

        return subjectCourse;
    }
}