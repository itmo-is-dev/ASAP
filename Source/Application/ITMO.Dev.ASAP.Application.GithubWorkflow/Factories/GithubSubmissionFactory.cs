using ITMO.Dev.ASAP.Application.Abstractions.Submissions;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Extensions;
using ITMO.Dev.ASAP.Core.Specifications.GroupAssignments;
using ITMO.Dev.ASAP.Core.Specifications.Submissions;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.SubjectCourseAssociations;
using ITMO.Dev.ASAP.Core.SubmissionAssociations;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.Core.Tools;
using ITMO.Dev.ASAP.Core.UserAssociations;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ITMO.Dev.ASAP.Application.GithubWorkflow.Factories;

public class GithubSubmissionFactory : ISubmissionFactory
{
    private readonly IDatabaseContext _context;

    private readonly string _organizationName;
    private readonly string _payload;
    private readonly long _pullRequestNumber;
    private readonly string _repositoryName;

    public GithubSubmissionFactory(
        IDatabaseContext context,
        string organizationName,
        string repositoryName,
        long pullRequestNumber,
        string payload)
    {
        _context = context;
        _organizationName = organizationName;
        _repositoryName = repositoryName;
        _pullRequestNumber = pullRequestNumber;
        _payload = payload;
    }

    public async Task<Submission> CreateAsync(Guid userId, Guid assignmentId, CancellationToken cancellationToken)
    {
        Student? student = await _context.Assignments
            .Where(x => x.Id.Equals(assignmentId))
            .SelectMany(x => x.SubjectCourse.Groups)
            .SelectMany(x => x.StudentGroup.Students)
            .Where(x => x.UserId.Equals(userId))
            .SingleOrDefaultAsync(cancellationToken);

        // If issuer is not a student, check if it is mentor and find student corresponding to the repository
        if (student is null)
        {
            IQueryable<SubjectCourse> subjectCourseQuery = _context.SubjectCourseAssociations
                .OfType<GithubSubjectCourseAssociation>()
                .Where(x =>
                    x.GithubOrganizationName.ToLower().Equals(_organizationName.ToLower()))
                .Select(x => x.SubjectCourse);

            Mentor? mentor = await subjectCourseQuery
                .SelectMany(x => x.Mentors)
                .Where(x => x.UserId.Equals(userId))
                .SingleOrDefaultAsync(cancellationToken);

            if (mentor is not null)
            {
                student = await FindStudentByRepositoryName(_repositoryName, cancellationToken);
            }
            else
            {
                SubjectCourse? subjectCourse = await subjectCourseQuery.SingleOrDefaultAsync(cancellationToken);

                if (subjectCourse is null)
                    throw EntityNotFoundException.SubjectCourseForOrganizationNotFound(_organizationName);

                throw EntityNotFoundException.UserNotFoundInSubjectCourse(userId, subjectCourse.Title);
            }
        }

        var groupAssignmentSpec = new GetStudentGroupAssignment(student.UserId, assignmentId);

        GroupAssignment groupAssignment = await _context.GroupAssignments
            .WithSpecification(groupAssignmentSpec)
            .SingleAsync(cancellationToken);

        var studentAssignmentSubmissionsSpec = new GetStudentAssignmentSubmissions(
            student.UserId,
            assignmentId);

        int count = await _context.Submissions
            .WithSpecification(studentAssignmentSubmissionsSpec)
            .CountAsync(cancellationToken);

        var submission = new Submission(
            Guid.NewGuid(),
            count + 1,
            student,
            groupAssignment,
            Calendar.CurrentDateTime,
            _payload);

        var githubAssociation = new GithubSubmissionAssociation(
            Guid.NewGuid(),
            submission,
            _organizationName,
            _repositoryName,
            _pullRequestNumber);

        submission.AddAssociation(githubAssociation);

        return submission;
    }

    private async Task<Student> FindStudentByRepositoryName(
        string repositoryName,
        CancellationToken cancellationToken)
    {
        IQueryable<User> users = _context.UserAssociations
            .OfType<GithubUserAssociation>()
            .Where(x => x.GithubUsername.ToLower().Equals(repositoryName.ToLower()))
            .Select(x => x.User);

        Student? student = await _context.Students
            .Where(student => users.Any(user => user.Id.Equals(student.UserId)))
            .SingleOrDefaultAsync(cancellationToken);

        if (student is null)
            throw new EntityNotFoundException("Student not found");

        return student;
    }
}