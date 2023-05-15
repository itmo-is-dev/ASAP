using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Github.Application.Contracts.SubjectCourses.Commands;
using ITMO.Dev.ASAP.Github.Application.Contracts.SubjectCourses.Notifications;
using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Github.Application.Octokit.Services;
using ITMO.Dev.ASAP.Github.Application.Specifications;
using ITMO.Dev.ASAP.Github.Common.Exceptions.Entities;
using ITMO.Dev.ASAP.Github.Common.Extensions;
using ITMO.Dev.ASAP.Github.Domain.SubjectCourses;
using ITMO.Dev.ASAP.Github.Domain.Users;
using ITMO.Dev.ASAP.Presentation.Contracts.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ITMO.Dev.ASAP.Github.Application.Handlers.SubjectCourse;

internal class SyncGithubMentorsHandler :
    IRequestHandler<SyncGithubMentors.Command>,
    INotificationHandler<SubjectCourseMentorTeamUpdated.Notification>
{
    private readonly ILogger<SyncGithubMentorsHandler> _logger;
    private readonly IGithubOrganizationService _githubOrganizationService;
    private readonly IAsapUserService _asapUserService;
    private readonly IGithubUserService _githubUserService;
    private readonly IAsapSubjectCourseService _asapSubjectCourseService;
    private readonly IPersistenceContext _context;

    public SyncGithubMentorsHandler(
        ILogger<SyncGithubMentorsHandler> logger,
        IGithubOrganizationService githubOrganizationService,
        IAsapUserService asapUserService,
        IGithubUserService githubUserService,
        IAsapSubjectCourseService asapSubjectCourseService,
        IPersistenceContext context)
    {
        _logger = logger;
        _githubOrganizationService = githubOrganizationService;
        _asapUserService = asapUserService;
        _githubUserService = githubUserService;
        _asapSubjectCourseService = asapSubjectCourseService;
        _context = context;
    }

    public async Task Handle(SyncGithubMentors.Command request, CancellationToken cancellationToken)
    {
        GithubSubjectCourse? subjectCourse = await _context.SubjectCourses
            .ForOrganizationName(request.OrganizationName, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (subjectCourse is null)
            throw EntityNotFoundException.SubjectCourse(request.OrganizationName);

        await UpdateMentorsAsync(subjectCourse, cancellationToken);
    }

    public async Task Handle(
        SubjectCourseMentorTeamUpdated.Notification notification,
        CancellationToken cancellationToken)
    {
        GithubSubjectCourse association = await _context.SubjectCourses
            .GetByIdAsync(notification.SubjectCourseId, cancellationToken);

        await UpdateMentorsAsync(association, cancellationToken);
    }

    private async Task UpdateMentorsAsync(
        GithubSubjectCourse subjectCourse,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Started updating github mentors for subject course {SubjectCourseId}",
            subjectCourse.Id);

        IReadOnlyCollection<string> mentorsTeamMembers = await _githubOrganizationService
            .GetTeamMemberUsernamesAsync(
                subjectCourse.OrganizationName,
                subjectCourse.MentorTeamName,
                cancellationToken);

        var exitingUsersQuery = GithubUserQuery.Build(x => x.WithUsernames(mentorsTeamMembers));

        List<GithubUser> existingGithubUsers = await _context.Users
            .QueryAsync(exitingUsersQuery, cancellationToken)
            .ToListAsync(cancellationToken);

        IEnumerable<string> existingGithubUsernames = existingGithubUsers.Select(x => x.Username);

        List<GithubUser> createdUsers = await mentorsTeamMembers
            .Except(existingGithubUsernames)
            .ToAsyncEnumerable()
            .SelectAwait(async username =>
            {
                bool userExists = await _githubUserService.IsUserExistsAsync(username, default);

                if (userExists is false)
                    throw EntityNotFoundException.Create<string, GithubUser>(username).TaggedWithNotFound();

                UserDto user = await _asapUserService.CreateUserAsync(username, username, username, default);
                return new GithubUser(user.Id, username);
            })
            .ToListAsync(default);

        _context.Users.AddRange(createdUsers);
        await _context.CommitAsync(cancellationToken);

        Guid[] userIds = existingGithubUsers.Concat(createdUsers).Select(x => x.Id).ToArray();
        await _asapSubjectCourseService.UpdateMentorsAsync(subjectCourse.Id, userIds, default);

        _logger.LogInformation("Updated github mentors for subject course {SubjectCourseId}", subjectCourse.Id);
    }
}