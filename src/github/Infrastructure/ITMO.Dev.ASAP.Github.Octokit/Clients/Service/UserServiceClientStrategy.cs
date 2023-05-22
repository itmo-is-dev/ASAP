using Octokit;

namespace ITMO.Dev.ASAP.Github.Octokit.Clients.Service;

public class UserServiceClientStrategy : IServiceClientStrategy
{
    private readonly IGitHubClient _client;
    private readonly string _username;

    public UserServiceClientStrategy(IGitHubClient client, string username)
    {
        _client = client;
        _username = username;
    }

    public async ValueTask<long> GetInstallationId()
    {
        Installation installation = await _client.GitHubApps.GetUserInstallationForCurrent(_username);
        return installation.Id;
    }
}