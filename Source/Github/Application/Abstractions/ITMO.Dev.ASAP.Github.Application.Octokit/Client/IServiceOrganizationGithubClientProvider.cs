using Octokit;

namespace ITMO.Dev.ASAP.Github.Application.Octokit.Client;

public interface IServiceOrganizationGithubClientProvider
{
    Task<GitHubClient> GetClient();
}