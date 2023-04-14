using ITMO.Dev.ASAP.Github.Application.Octokit.Client;
using ITMO.Dev.ASAP.Github.Application.Octokit.Configurations;
using Microsoft.Extensions.Options;
using Octokit;

namespace ITMO.Dev.ASAP.Github.Octokit.Client;

public class ServiceOrganizationGithubClientProvider : IServiceOrganizationGithubClientProvider
{
    private readonly IGitHubClient _appClient;
    private readonly IInstallationClientFactory _clientFactory;
    private readonly string _serviceOrganization;

    public ServiceOrganizationGithubClientProvider(
        IOptions<GithubIntegrationConfiguration> configuration,
        IGitHubClient appClient,
        IInstallationClientFactory clientFactory)
    {
        _appClient = appClient;
        _clientFactory = clientFactory;
        _serviceOrganization = configuration.Value.GithubAppConfiguration.ServiceOrganizationName;
    }

    public async Task<GitHubClient> GetClient()
    {
        Installation installation = await _appClient.GitHubApps
            .GetOrganizationInstallationForCurrent(_serviceOrganization);

        return _clientFactory.GetClient(installation.Id);
    }
}