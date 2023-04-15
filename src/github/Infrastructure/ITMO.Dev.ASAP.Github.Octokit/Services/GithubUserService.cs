using ITMO.Dev.ASAP.Github.Application.Octokit.Client;
using ITMO.Dev.ASAP.Github.Application.Octokit.Services;
using Octokit;

namespace ITMO.Dev.ASAP.Github.Octokit.Services;

public class GithubUserService : IGithubUserService
{
    private readonly IServiceOrganizationGithubClientProvider _clientProvider;

    public GithubUserService(IServiceOrganizationGithubClientProvider clientProvider)
    {
        _clientProvider = clientProvider;
    }

    public async Task<bool> IsUserExistsAsync(string username, CancellationToken cancellationToken)
    {
        GitHubClient client = await _clientProvider.GetClient();

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