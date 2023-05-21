using Octokit;

namespace ITMO.Dev.ASAP.Github.Octokit.Clients.Service;

public class OrganizationServiceClientStrategy : IServiceClientStrategy
{
    private readonly IGitHubClient _client;
    private readonly string _organizationName;

    public OrganizationServiceClientStrategy(IGitHubClient client, string organizationName)
    {
        _client = client;
        _organizationName = organizationName;
    }

    public async ValueTask<long> GetInstallationId()
    {
        Installation installation = await _client.GitHubApps.GetOrganizationInstallationForCurrent(_organizationName);
        return installation.Id;
    }
}