using ITMO.Dev.ASAP.Github.Application.Octokit.Clients;
using ITMO.Dev.ASAP.Github.Application.Octokit.Services;
using Octokit;

namespace ITMO.Dev.ASAP.Github.Octokit.Services;

public class GithubUserService : IGithubUserService
{
    private readonly IGithubClientProvider _clientProvider;

    public GithubUserService(IGithubClientProvider clientProvider)
    {
        _clientProvider = clientProvider;
    }

    public async Task<bool> IsUserExistsAsync(string username, CancellationToken cancellationToken)
    {
        IGitHubClient client = await _clientProvider.GetClientAsync(cancellationToken);

        try
        {
            User user = await client.User.Get(username);
            return user.Login.Equals(username, StringComparison.OrdinalIgnoreCase);
        }
        catch (NotFoundException)
        {
            return false;
        }
    }
}