using ITMO.Dev.ASAP.Application.Abstractions.Permissions;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Github.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using Microsoft.EntityFrameworkCore;

namespace ITMO.Dev.ASAP.Application.Validators;

public class PermissionValidator : IPermissionValidator
{
    private readonly IDatabaseContext _context;
    private readonly IGithubSubjectCourseService _githubSubjectCourseService;

    public PermissionValidator(IDatabaseContext context, IGithubSubjectCourseService githubSubjectCourseService)
    {
        _context = context;
        _githubSubjectCourseService = githubSubjectCourseService;
    }

    public static bool IsRepositoryOwner(string username, string repositoryName)
    {
        return string.Equals(username, repositoryName, StringComparison.OrdinalIgnoreCase);
    }

    public async Task<bool> IsOrganizationMentorAsync(
        Guid senderId,
        string organizationName,
        CancellationToken cancellationToken)
    {
        GithubSubjectCourseDto subjectCourse = await _githubSubjectCourseService
            .GetByOrganizationName(organizationName, cancellationToken);

        return await _context.SubjectCourses
            .WithId(subjectCourse.Id)
            .IsMentor(senderId)
            .AnyAsync(cancellationToken);
    }

    public Task<bool> IsSubmissionMentorAsync(Guid userId, Guid submissionId, CancellationToken cancellationToken)
    {
        return _context.Submissions
            .Where(x => x.Id.Equals(submissionId))
            .Select(x => x.GroupAssignment.Assignment.SubjectCourse)
            .SelectMany(x => x.Mentors)
            .AnyAsync(x => x.UserId.Equals(userId), cancellationToken);
    }

    public async Task EnsureSubmissionMentorAsync(
        Guid userId,
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        if (await IsSubmissionMentorAsync(userId, submissionId, cancellationToken) is false)
            throw new UnauthorizedException();
    }
}