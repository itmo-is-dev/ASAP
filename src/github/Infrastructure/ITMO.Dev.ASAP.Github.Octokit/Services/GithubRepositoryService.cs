using ITMO.Dev.ASAP.Github.Application.Octokit.Client;
using ITMO.Dev.ASAP.Github.Application.Octokit.Models;
using ITMO.Dev.ASAP.Github.Application.Octokit.Services;
using Microsoft.Extensions.Logging;
using Octokit;

namespace ITMO.Dev.ASAP.Github.Octokit.Services;

public class GithubRepositoryService : IGithubRepositoryService
{
    private readonly IOrganizationGithubClientProvider _clientProvider;
    private readonly ILogger<GithubRepositoryService> _logger;

    public GithubRepositoryService(
        IOrganizationGithubClientProvider clientProvider,
        ILogger<GithubRepositoryService> logger)
    {
        _clientProvider = clientProvider;
        _logger = logger;
    }

    public async Task AddTeamPermission(string organization, string repositoryName, Team team, Permission permission)
    {
        GitHubClient client = await _clientProvider.GetClient(organization);

        _logger.LogInformation(
            "Adding permission {Permission} for {Team} in {OrganizationName}/{RepositoryName}",
            permission,
            team.Name,
            organization,
            repositoryName);

        await client.Organization.Team.AddRepository(
            team.Id,
            organization,
            repositoryName,
            new RepositoryPermissionRequest(permission));
    }

    public async Task CreateRepositoryFromTemplate(string organization, string newRepositoryName, string templateName)
    {
        GitHubClient client = await _clientProvider.GetClient(organization);

        var userRepositoryFromTemplate = new NewRepositoryFromTemplate(newRepositoryName)
        {
            Owner = organization, Description = null, Private = true,
        };

        _logger.LogInformation(
            "Creating repository {OrganizationName}/{RepositoryName} from {Template}",
            organization,
            newRepositoryName,
            templateName);

        await client.Repository.Generate(
            organization,
            templateName,
            userRepositoryFromTemplate);
    }

    public async Task<AddPermissionResult> AddUserPermission(
        string organization,
        string repositoryName,
        string username,
        Permission permission)
    {
        GitHubClient client = await _clientProvider.GetClient(organization);

        RepositoryInvitation invitation = await client.Repository.Collaborator.Invite(
            organization,
            repositoryName,
            username,
            new CollaboratorRequest(permission));

        if (invitation is null)
        {
            bool isCollaborator = await client.Repository.Collaborator.IsCollaborator(
                organization,
                repositoryName,
                username);

            if (isCollaborator)
                return AddPermissionResult.AlreadyCollaborator;

            _logger.LogInformation(
                "Adding permission {Permission} for {Username} in {OrganizationName}/{RepositoryName}",
                permission,
                username,
                organization,
                username);

            await client.Repository.Collaborator.Add(
                organization,
                repositoryName,
                username,
                new CollaboratorRequest(permission));

            return AddPermissionResult.Invited;
        }

        if (DateTimeOffset.UtcNow.Subtract(invitation.CreatedAt) < TimeSpan.FromDays(7))
            return AddPermissionResult.Pending;

        _logger.LogInformation(
            "Invitation for {Username} in {OrganizationName}/{RepositoryName} is expired, renewing",
            username,
            organization,
            repositoryName);

        Repository repository = await client.Repository.Get(organization, repositoryName);
        await client.Repository.Invitation.Delete(repository.Id, invitation.Id);

        await client.Repository.Collaborator.Add(
            organization,
            repositoryName,
            username,
            new CollaboratorRequest(permission));

        return AddPermissionResult.ReInvited;
    }
}