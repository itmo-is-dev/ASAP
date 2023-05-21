using ITMO.Dev.ASAP.Github.Application.Octokit.Clients;
using ITMO.Dev.ASAP.Github.Application.Octokit.Services;
using ITMO.Dev.ASAP.Github.Common.Exceptions.Entities;
using ITMO.Dev.ASAP.Github.Common.Extensions;
using Octokit;

namespace ITMO.Dev.ASAP.Github.Octokit.Services;

public class GithubOrganizationService : IGithubOrganizationService
{
    private readonly IGithubClientProvider _clientProvider;

    public GithubOrganizationService(IGithubClientProvider clientProvider)
    {
        _clientProvider = clientProvider;
    }

    public async Task<IReadOnlyCollection<string>> GetTeamMemberUsernamesAsync(
        string organizationName,
        string teamName,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IGitHubClient client = await _clientProvider
            .GetClientForOrganizationAsync(organizationName, cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<Team> teams = await client.Organization.Team.GetAll(organizationName);

        Team? team = teams.FirstOrDefault(t => t.Name == teamName);

        if (team is null)
            throw EntityNotFoundException.Create<string, Team>(teamName).TaggedWithBadRequest();

        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<User> teamMembers = await client.Organization.Team.GetAllMembers(team.Id);

        return teamMembers.Select(u => u.Login).ToArray();
    }

    public async Task<IReadOnlyCollection<string>> GetRepositoriesAsync(
        string organizationName,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IGitHubClient client = await _clientProvider
            .GetClientForOrganizationAsync(organizationName, cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<Repository> repositories = await client.Repository.GetAllForOrg(organizationName);

        return repositories.Select(repository => repository.Name).ToArray();
    }

    public async Task<Team> GetTeamAsync(string organizationName, string teamName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IGitHubClient client = await _clientProvider
            .GetClientForOrganizationAsync(organizationName, cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<Team> teams = await client.Organization.Team.GetAll(organizationName);

        return teams.Single(x => teamName.Equals(x.Name, StringComparison.OrdinalIgnoreCase));
    }
}