using Octokit;

namespace ITMO.Dev.ASAP.Github.Application.Octokit.Client;

public interface IOrganizationGithubClientProvider
{
    Task<GitHubClient> GetClient(string organization);
}