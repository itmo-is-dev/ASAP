using ITMO.Dev.ASAP.Github.Application.Extensions;
using ITMO.Dev.ASAP.Github.Application.Handlers.Extensions;
using ITMO.Dev.ASAP.Github.Caching.Extensions;
using ITMO.Dev.ASAP.Github.DataAccess.Extensions;
using ITMO.Dev.ASAP.Github.Octokit.Extensions;
using ITMO.Dev.ASAP.Github.Presentation.Services.Extensions;
using ITMO.Dev.ASAP.Github.Presentation.Webhooks.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Github;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAsapGithub(
        this IServiceCollection collection,
        IConfiguration configuration,
        string databaseConnectionString)
    {
        bool enabled = configuration.GetValue<bool>("Github:Enabled");

        if (enabled)
        {
            collection.AddGithubApplication(configuration);
            collection.AddGithubApplicationHandlers();
            collection.AddGithubCaching(configuration);

            collection.AddGithubOctokitIntegration(configuration);

            collection.AddGithubDatabaseContext(databaseConnectionString);

            collection
                .AddGithubPresentation(configuration)
                .AddGithubPresentationServices();
        }
        else
        {
            collection.AddDummyGithubPresentationServices();
        }

        return collection;
    }
}