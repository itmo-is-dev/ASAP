using ITMO.Dev.ASAP.Github.Application.Contracts.SubjectCourses.Commands;
using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Extensions;
using ITMO.Dev.ASAP.Github.Application.Octokit.Models;
using ITMO.Dev.ASAP.Github.Application.Octokit.Services;
using ITMO.Dev.ASAP.Github.Application.Specifications;
using ITMO.Dev.ASAP.Github.Domain.SubjectCourses;
using ITMO.Dev.ASAP.Github.Domain.Users;
using ITMO.Dev.ASAP.Presentation.Contracts.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Octokit;

namespace ITMO.Dev.ASAP.Github.Application.Handlers.SubjectCourse;

internal class SubjectCourseOrganizationUpdateHandler
    : IRequestHandler<UpdateSubjectCourseOrganizations.Command>,
        IRequestHandler<UpdateSubjectCourseOrganization.Command>
{
    private readonly ILogger<SubjectCourseOrganizationUpdateHandler> _logger;
    private readonly IDatabaseContext _context;
    private readonly IAsapSubjectCourseService _asapSubjectCourseService;
    private readonly IGithubOrganizationService _githubOrganizationService;
    private readonly IGithubRepositoryService _githubRepositoryService;

    public SubjectCourseOrganizationUpdateHandler(
        ILogger<SubjectCourseOrganizationUpdateHandler> logger,
        IDatabaseContext context,
        IAsapSubjectCourseService asapSubjectCourseService,
        IGithubOrganizationService githubOrganizationService,
        IGithubRepositoryService githubRepositoryService)
    {
        _logger = logger;
        _context = context;
        _asapSubjectCourseService = asapSubjectCourseService;
        _githubOrganizationService = githubOrganizationService;
        _githubRepositoryService = githubRepositoryService;
    }

    public async Task<Unit> Handle(UpdateSubjectCourseOrganizations.Command request, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<GithubSubjectCourse> subjectCourses = await _context.SubjectCourses
            .ToListAsync(cancellationToken);

        foreach (GithubSubjectCourse subjectCourse in subjectCourses)
        {
            await UpdateOrganizationAsync(subjectCourse, cancellationToken);
        }

        return Unit.Value;
    }

    public async Task<Unit> Handle(UpdateSubjectCourseOrganization.Command request, CancellationToken cancellationToken)
    {
        GithubSubjectCourse subjectCourse = await _context.SubjectCourses
            .GetByIdAsync(request.SubjectCourseId, cancellationToken);

        await UpdateOrganizationAsync(subjectCourse, cancellationToken);

        return Unit.Value;
    }

    private async Task UpdateOrganizationAsync(GithubSubjectCourse subjectCourse, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Guid> studentIds = await _asapSubjectCourseService
            .GetSubjectCourseStudentIds(subjectCourse.Id, cancellationToken);

        IReadOnlyCollection<GithubUser> githubUsers = await _context.Users
            .WithIds(studentIds)
            .ToListAsync(cancellationToken);

        string[] usernames = githubUsers.Select(a => a.Username).ToArray();

        _logger.LogInformation(
            "Started repository generation for organization {OrganizationName}",
            subjectCourse.OrganizationName);

        await GenerateRepositories(usernames, subjectCourse, cancellationToken);

        _logger.LogInformation(
            "Finished repository generation for organization {OrganizationName}",
            subjectCourse.OrganizationName);
    }

    private async ValueTask GenerateRepositories(
        IEnumerable<string> usernames,
        GithubSubjectCourse association,
        CancellationToken cancellationToken)
    {
        string organizationName = association.OrganizationName;
        string templateName = association.TemplateRepositoryName;
        string mentorTeamName = association.MentorTeamName;

        IReadOnlyCollection<string> repositories = await _githubOrganizationService
            .GetRepositoriesAsync(organizationName, cancellationToken);

        if (repositories.Contains(templateName) is false)
        {
            _logger.LogWarning("No template repository found for organization {OrganizationName}", organizationName);
            return;
        }

        Team team = await _githubOrganizationService.GetTeamAsync(organizationName, mentorTeamName, cancellationToken);

        foreach (string username in usernames)
        {
            if (repositories.Any(r => r.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                await TryAddUserPermissionsAsync(organizationName, username);
                continue;
            }

            if (await TryCreateRepositoryFromTemplateAsync(organizationName, templateName, username) is false)
                continue;

            await AddTeamPermissionAsync(organizationName, username, team);

            if (await TryAddUserPermissionsAsync(organizationName, username) is false)
                continue;

            _logger.LogInformation("Successfully created repository for user {User}", username);
        }
    }

    private async Task AddTeamPermissionAsync(string organizationName, string username, Team team)
    {
        const Permission permission = Permission.Maintain;
        await _githubRepositoryService.AddTeamPermission(organizationName, username, team, permission);
    }

    private async Task<bool> TryCreateRepositoryFromTemplateAsync(
        string organizationName,
        string templateName,
        string username)
    {
        try
        {
            await _githubRepositoryService.CreateRepositoryFromTemplate(organizationName, username, templateName);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create repo for {Username}", username);
            return false;
        }
    }

    private async Task<bool> TryAddUserPermissionsAsync(
        string organizationName,
        string username)
    {
        const Permission permission = Permission.Push;

        try
        {
            AddPermissionResult result = await _githubRepositoryService.AddUserPermission(
                organizationName,
                username,
                username,
                permission);

            return result is AddPermissionResult.Invited;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to add user {Username} to repo", username);
            return false;
        }
    }
}