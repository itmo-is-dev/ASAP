using Bogus;
using ITMO.Dev.ASAP.Core.Models;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.SubmissionAssociations;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.Core.Tools;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.Core.ValueObject;
using ITMO.Dev.ASAP.Seeding.Extensions;
using ITMO.Dev.ASAP.Seeding.Options;

namespace ITMO.Dev.ASAP.Seeding.EntityGenerators.Submissions;

public class SubmissionGenerator : EntityGeneratorBase<Submission>
{
    private const double MaxExtraPoints = 15;
    private const float ExtraPointsPresenceProbability = 0.1f;
    private readonly IEntityGenerator<Assignment> _assignmentGenerator;

    private readonly Faker _faker;

    public SubmissionGenerator(
        EntityGeneratorOptions<Submission> options,
        Faker faker,
        IEntityGenerator<Assignment> assignmentGenerator)
        : base(options)
    {
        _faker = faker;
        _assignmentGenerator = assignmentGenerator;
    }

    protected override Submission Generate(int index)
    {
        IEnumerable<Assignment> assignments = _assignmentGenerator.GeneratedEntities
            .Where(x => x.GroupAssignments.SelectMany(xx => xx.Group.Students).Any());

        Assignment assignment = _faker.PickRandom(assignments);

        IEnumerable<Student> students = assignment.GroupAssignments.SelectMany(x => x.Group.Students)
            .Where(student => assignment.SubjectCourse.Mentors.Any(mentor => mentor.User.Equals(student.User)) is false);

        Student student = _faker.PickRandom(students);

        GroupAssignment groupAssignment = assignment.GroupAssignments.Single(x => x.Group.Equals(student.Group));

        int submissionCount = groupAssignment.Submissions.Count(x => x.Student.Equals(student));

        var submission = new Submission(
            _faker.Random.Guid(),
            submissionCount + 1,
            student,
            groupAssignment,
            Calendar.FromLocal(_faker.Date.Future()),
            _faker.Internet.Url());

        var githubAssociation = new GithubSubmissionAssociation(
            _faker.Random.Guid(),
            submission,
            _faker.Company.CompanyName(),
            _faker.Commerce.ProductName(),
            _faker.Random.Long(0, 100));

        submission.AddAssociation(githubAssociation);

        groupAssignment.AddSubmission(submission);

        SubmissionStateKind stateKind = _faker.PickRandom(Enum.GetValues<SubmissionStateKind>());

        SetSubmissionState(stateKind, submission);

        return submission;
    }

    private void SetSubmissionState(SubmissionStateKind stateKind, Submission submission)
    {
        switch (stateKind)
        {
            case SubmissionStateKind.Active:
                break;

            case SubmissionStateKind.Inactive:
                submission.Deactivate();
                break;

            case SubmissionStateKind.Deleted:
                submission.Delete();
                break;

            case SubmissionStateKind.Completed:
                Fraction rating = _faker.Random.Fraction();

                Points? extraPoints = _faker.Random.Bool(ExtraPointsPresenceProbability)
                    ? _faker.Random.Points(0, MaxExtraPoints)
                    : Points.None;

                submission.Rate(rating, extraPoints);
                break;

            case SubmissionStateKind.Reviewed:
                submission.MarkAsReviewed();
                break;

            case SubmissionStateKind.Banned:
                submission.Ban();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(stateKind));
        }
    }
}