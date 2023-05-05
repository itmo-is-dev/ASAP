using ITMO.Dev.ASAP.Github.Application.Extensions;
using ITMO.Dev.ASAP.Github.Application.Handlers.Extensions;
using ITMO.Dev.ASAP.Github.DataAccess.Extensions;
using ITMO.Dev.ASAP.Github.Octokit.Extensions;
using ITMO.Dev.ASAP.Github.Presentation.Services.Extensions;
using ITMO.Dev.ASAP.Github.Presentation.Webhooks.Extensions;
using Microsoft.EntityFrameworkCore;
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
        collection.AddGithubApplication();
        collection.AddGithubApplicationHandlers();

        collection.AddGithubOctokitIntegration(configuration);

        collection.AddGithubDatabaseContext(x => x
            .UseNpgsql(databaseConnectionString)
            .UseLazyLoadingProxies());

        collection
            .AddGithubPresentation()
            .AddGithubPresentationServices();

        return collection;
    }
}