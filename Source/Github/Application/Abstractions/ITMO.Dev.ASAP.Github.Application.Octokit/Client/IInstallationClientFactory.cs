using Octokit;

namespace ITMO.Dev.ASAP.Github.Application.Octokit.Client;

public interface IInstallationClientFactory
{
    GitHubClient GetClient(long installationId);
}