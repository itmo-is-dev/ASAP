using GitHubJwt;
using ITMO.Dev.ASAP.Github.Application.Octokit.Client;
using ITMO.Dev.ASAP.Github.Application.Octokit.Configurations;
using ITMO.Dev.ASAP.Github.Application.Octokit.Services;
using ITMO.Dev.ASAP.Github.Octokit.Caching;
using ITMO.Dev.ASAP.Github.Octokit.Client;
using ITMO.Dev.ASAP.Github.Octokit.CredentialStores;
using ITMO.Dev.ASAP.Github.Octokit.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Octokit;

namespace ITMO.Dev.ASAP.Github.Octokit.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<CacheConfiguration>(configuration.GetSection(nameof(CacheConfiguration)));

        services.Configure<GithubIntegrationConfiguration>(
            configuration.GetSection(nameof(GithubIntegrationConfiguration)));

        services.AddClientFactory();
        services.AddGithubServices();

        return services;
    }

    private static IServiceCollection AddClientFactory(this IServiceCollection services)
    {
        services.AddSingleton(provider =>
        {
            IOptions<GithubIntegrationConfiguration> configuration = provider
                .GetRequiredService<IOptions<GithubIntegrationConfiguration>>();

            var privateKeySource = new FullStringPrivateKeySource(configuration.Value.GithubAppConfiguration.PrivateKey);

            var jwtFactoryOptions = new GitHubJwtFactoryOptions
            {
                // The GitHub App Id
                AppIntegrationId = configuration.Value.GithubAppConfiguration.AppIntegrationId,

                // 10 minutes is the maximum time allowed
                ExpirationSeconds = configuration.Value.GithubAppConfiguration.JwtExpirationSeconds,
            };

            return new GitHubJwtFactory(privateKeySource, jwtFactoryOptions);
        });

        services.AddSingleton<IAsapMemoryCache, AsapMemoryCache>(provider =>
        {
            IOptions<CacheConfiguration> configuration = provider.GetRequiredService<IOptions<CacheConfiguration>>();

            var memoryCacheOptions = new MemoryCacheOptions
            {
                SizeLimit = configuration.Value.SizeLimit,
                ExpirationScanFrequency = configuration.Value.Expiration,
            };

            MemoryCacheEntryOptions memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(configuration.Value.CacheEntryConfiguration.EntrySize)
                .SetAbsoluteExpiration(configuration.Value.CacheEntryConfiguration.AbsoluteExpiration)
                .SetSlidingExpiration(configuration.Value.CacheEntryConfiguration.SlidingExpiration);

            return new AsapMemoryCache(memoryCacheOptions, memoryCacheEntryOptions);
        });

        services.AddSingleton<IGitHubClient>(serviceProvider =>
        {
            GitHubJwtFactory githubJwtFactory = serviceProvider.GetService<GitHubJwtFactory>()!;

            return new GitHubClient(
                new ProductHeaderValue("ITMO.Dev.ASAP"),
                new GithubAppCredentialStore(githubJwtFactory));
        });

        services.AddSingleton<IInstallationClientFactory, InstallationClientFactory>();
        services.AddSingleton<IOrganizationGithubClientProvider, OrganizationGithubClientProvider>();
        services.AddSingleton<IServiceOrganizationGithubClientProvider, ServiceOrganizationGithubClientProvider>();

        return services;
    }

    private static IServiceCollection AddGithubServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IGithubUserService, GithubUserService>()
            .AddScoped<IGithubOrganizationService, GithubOrganizationService>()
            .AddScoped<IGithubUserService, GithubUserService>();
    }
}