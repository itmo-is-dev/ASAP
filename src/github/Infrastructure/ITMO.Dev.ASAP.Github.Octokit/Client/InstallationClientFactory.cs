using ITMO.Dev.ASAP.Github.Application.Octokit.Client;
using ITMO.Dev.ASAP.Github.Octokit.Caching;
using ITMO.Dev.ASAP.Github.Octokit.CredentialStores;
using Octokit;

namespace ITMO.Dev.ASAP.Github.Octokit.Client;

public class InstallationClientFactory : IInstallationClientFactory
{
    private readonly IGitHubClient _gitHubAppClient;
    private readonly IAsapMemoryCache _memoryCache;

    public InstallationClientFactory(IGitHubClient gitHubAppClient, IAsapMemoryCache memoryCache)
    {
        _gitHubAppClient = gitHubAppClient;
        _memoryCache = memoryCache;
    }

    public GitHubClient GetClient(long installationId)
    {
        return _memoryCache.GetOrCreate(installationId, _ => CreateInstallationClient(installationId));
    }

    private GitHubClient CreateInstallationClient(long installationId)
    {
        return new GitHubClient(
            new ProductHeaderValue($"Installation-{installationId}"),
            new InstallationCredentialStore(_gitHubAppClient, installationId));
    }
}