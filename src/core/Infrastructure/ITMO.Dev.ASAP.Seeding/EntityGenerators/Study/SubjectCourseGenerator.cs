using Bogus;
using ITMO.Dev.ASAP.Domain.DeadlinePolicies;
using ITMO.Dev.ASAP.Domain.SubmissionStateWorkflows;
using ITMO.Dev.ASAP.Seeding.Options;
using Subject = ITMO.Dev.ASAP.Domain.Study.Subject;
using SubjectCourse = ITMO.Dev.ASAP.Domain.Study.SubjectCourse;
using User = ITMO.Dev.ASAP.Domain.Users.User;

namespace ITMO.Dev.ASAP.Seeding.EntityGenerators;

public class SubjectCourseGenerator : EntityGeneratorBase<SubjectCourse>
{
    private readonly IEntityGenerator<DeadlinePolicy> _deadlinePolicyGenerator;
    private readonly Faker _faker;
    private readonly IEntityGenerator<Subject> _subjectGenerator;
    private readonly IEntityGenerator<User> _userGenerator;

    public SubjectCourseGenerator(
        EntityGeneratorOptions<SubjectCourse> options,
        IEntityGenerator<User> userGenerator,
        IEntityGenerator<Subject> subjectGenerator,
        Faker faker,
        IEntityGenerator<DeadlinePolicy> deadlinePolicyGenerator)
        : base(options)
    {
        _userGenerator = userGenerator;
        _subjectGenerator = subjectGenerator;
        _faker = faker;
        _deadlinePolicyGenerator = deadlinePolicyGenerator;
    }

    protected override SubjectCourse Generate(int index)
    {
        int subjectCount = _subjectGenerator.GeneratedEntities.Count;

        int deadlineCount = _faker.Random.Int(0, _deadlinePolicyGenerator.GeneratedEntities.Count);

        IEnumerable<DeadlinePolicy> deadlines = Enumerable.Range(0, deadlineCount)
            .Select(_ => _faker.Random.Int(0, _deadlinePolicyGenerator.GeneratedEntities.Count - 1))
            .Select(i => _deadlinePolicyGenerator.GeneratedEntities[i])
            .Distinct();

        if (index >= subjectCount)
            throw new IndexOutOfRangeException("The subject index is greater than the number of subjects.");

        Subject subject = _subjectGenerator.GeneratedEntities[index];

        string? subjectCourseName = _faker.Commerce.ProductName();

        const SubmissionStateWorkflowType reviewType = SubmissionStateWorkflowType.ReviewWithDefense;

        var subjectCourse = new SubjectCourse(_faker.Random.Guid(), subject, subjectCourseName, reviewType);

        IEnumerable<User> users = _faker.Random
            .ListItems(_userGenerator.GeneratedEntities.ToList(), 2)
            .Distinct();

        foreach (User user in users)
        {
            subjectCourse.AddMentor(user);
        }

        foreach (DeadlinePolicy deadline in deadlines)
        {
            subjectCourse.AddDeadlinePolicy(deadline);
        }

        subject.AddCourse(subjectCourse);

        return subjectCourse;
    }
}