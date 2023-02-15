using ITMO.Dev.ASAP.Application.DatabaseContextExtensions;
using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions;
using ITMO.Dev.ASAP.Core.SubjectCourseAssociations;
using ITMO.Dev.ASAP.Core.UserAssociations;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Octokit;

namespace ITMO.Dev.ASAP.Application.GithubWorkflow.OrganizationManagement;

public class SubjectCourseGithubOrganizationManager : ISubjectCourseGithubOrganizationManager
{
    private readonly IDatabaseContext _context;
    private readonly ISubjectCourseGithubOrganizationInviteSender _inviteSender;
    private readonly ILogger<SubjectCourseGithubOrganizationManager> _logger;
    private readonly ISubjectCourseGithubOrganizationRepositoryManager _repositoryManager;

    public SubjectCourseGithubOrganizationManager(
        ISubjectCourseGithubOrganizationInviteSender inviteSender,
        ISubjectCourseGithubOrganizationRepositoryManager repositoryManager,
        IDatabaseContext context,
        ILogger<SubjectCourseGithubOrganizationManager> logger)
    {
        _inviteSender = inviteSender;
        _repositoryManager = repositoryManager;
        _context = context;
        _logger = logger;
    }

    public async Task UpdateOrganizations(CancellationToken cancellationToken)
    {
        List<GithubSubjectCourseAssociation> githubSubjectCourseAssociations = await _context
            .SubjectCourseAssociations
            .OfType<GithubSubjectCourseAssociation>()
            .ToListAsync(cancellationToken);

        foreach (GithubSubjectCourseAssociation subjectAssociation in githubSubjectCourseAssociations)
        {
            IReadOnlyCollection<GithubUserAssociation> githubUserAssociations = await _context.SubjectCourses
                .GetAllGithubUsers(subjectAssociation.SubjectCourse.Id);

            string[] usernames = githubUserAssociations.Select(a => a.GithubUsername).ToArray();

            await _inviteSender.Invite(subjectAssociation.GithubOrganizationName, usernames);

            await GenerateRepositories(usernames, subjectAssociation);
        }
    }

    private async ValueTask GenerateRepositories(
        IReadOnlyCollection<string> usernames,
        GithubSubjectCourseAssociation association)
    {
        string organizationName = association.GithubOrganizationName;
        string templateName = association.TemplateRepositoryName;
        string mentorTeamName = association.MentorTeamName;

        IReadOnlyCollection<string> repositories = await _repositoryManager.GetRepositories(organizationName);

        if (repositories.Contains(templateName) is false)
        {
            _logger.LogWarning("No template repository found for organization {OrganizationName}", organizationName);
            return;
        }

        Team team = await _repositoryManager.GetTeam(organizationName, mentorTeamName);

        foreach (string username in usernames)
        {
            if (repositories.Any(r => r.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                await ResendInviteIfNeeded(organizationName, username);
                continue;
            }

            if (await TryCreateRepositoryFromTemplateAsync(organizationName, templateName, username) is false)
                continue;

            await _repositoryManager.AddTeamPermission(organizationName, username, team, Permission.Maintain);

            if (await TryAddUserPermissionsAsync(organizationName, username) is false)
                continue;

            _logger.LogInformation("Successfully created repository for user {User}", username);
        }
    }

    private async Task ResendInviteIfNeeded(string organizationName, string repositoryName)
    {
        if (await _repositoryManager.IsRepositoryCollaborator(organizationName, repositoryName, repositoryName))
            return;

        await TryAddUserPermissionsAsync(organizationName, repositoryName);
    }

    private async Task<bool> TryCreateRepositoryFromTemplateAsync(
        string organizationName,
        string templateName,
        string username)
    {
        try
        {
            await _repositoryManager.CreateRepositoryFromTemplate(organizationName, username, templateName);
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
        try
        {
            await _repositoryManager.AddUserPermission(
                organizationName,
                username,
                username,
                Permission.Push);

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to add user {Username} to repo", username);
            return false;
        }
    }
}