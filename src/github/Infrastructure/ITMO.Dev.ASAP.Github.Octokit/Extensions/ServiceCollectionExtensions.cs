using GitHubJwt;
using ITMO.Dev.ASAP.Github.Application.Octokit.Clients;
using ITMO.Dev.ASAP.Github.Application.Octokit.Services;
using ITMO.Dev.ASAP.Github.Octokit.Clients;
using ITMO.Dev.ASAP.Github.Octokit.Configuration;
using ITMO.Dev.ASAP.Github.Octokit.Configuration.ServiceClients;
using ITMO.Dev.ASAP.Github.Octokit.CredentialStores;
using ITMO.Dev.ASAP.Github.Octokit.Services;
using ITMO.Dev.ASAP.Github.Octokit.Tools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Octokit;

namespace ITMO.Dev.ASAP.Github.Octokit.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubOctokitIntegration(
        this IServiceCollection collection,
        IConfiguration configuration)
    {
        IConfigurationSection section = configuration.GetSection("Github:Octokit");
        collection.Configure<GithubOctokitConfiguration>(section);
        collection.AddSingleton<IValidateOptions<GithubOctokitConfiguration>, GithubOctokitConfiguration>();

        collection.AddGithubClients(configuration);
        collection.AddGithubServices();

        return collection;
    }

    private static IServiceCollection AddGithubClients(this IServiceCollection collection, IConfiguration configuration)
    {
        collection.AddSingleton(provider =>
        {
            IOptions<GithubOctokitConfiguration> octokitConfiguration = provider
                .GetRequiredService<IOptions<GithubOctokitConfiguration>>();

            var privateKeySource = new FullStringPrivateKeySource(octokitConfiguration.Value.PrivateKey);

            var jwtFactoryOptions = new GitHubJwtFactoryOptions
            {
                // The GitHub App Id
                AppIntegrationId = octokitConfiguration.Value.AppId,

                // 10 minutes is the maximum time allowed
                ExpirationSeconds = octokitConfiguration.Value.JwtExpirationSeconds,
            };

            return new GitHubJwtFactory(privateKeySource, jwtFactoryOptions);
        });

        collection.AddSingleton<IGitHubClient>(serviceProvider =>
        {
            GitHubJwtFactory githubJwtFactory = serviceProvider.GetService<GitHubJwtFactory>()!;

            return new GitHubClient(
                new ProductHeaderValue("ITMO.Dev.ASAP"),
                new GithubAppCredentialStore(githubJwtFactory));
        });

        FluentChaining.IChain<ServiceClientCommand> serviceClientChain = FluentChaining.FluentChaining
            .CreateChain<ServiceClientCommand>(start => start
                .Then<InstallationServiceClientLink>()
                .Then<OrganizationServiceClientLink>()
                .Then<UserServiceClientLink>()
                .FinishWith(() => throw new InvalidOperationException("Please configure Github:Octokit:Service")));

        serviceClientChain.Process(new ServiceClientCommand(collection, configuration));

        collection.AddSingleton<IGithubClientProvider, GithubClientProvider>();

        return collection;
    }

    private static IServiceCollection AddGithubServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IGithubUserService, GithubUserService>()
            .AddScoped<IGithubOrganizationService, GithubOrganizationService>()
            .AddScoped<IGithubRepositoryService, GithubRepositoryService>();
    }
}