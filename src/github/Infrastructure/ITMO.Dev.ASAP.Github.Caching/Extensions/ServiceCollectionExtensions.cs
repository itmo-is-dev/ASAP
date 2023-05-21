using ITMO.Dev.ASAP.Github.Caching.Models;
using ITMO.Dev.ASAP.Github.Caching.Tools;
using ITMO.Dev.ASAP.Github.Common.Tools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Github.Caching.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubCaching(
        this IServiceCollection collection,
        IConfiguration configuration)
    {
        IConfigurationSection section = configuration.GetSection("Github:Cache");
        collection.Configure<GithubCacheConfiguration>(section);

        collection.AddSingleton<IGithubMemoryCache, GithubMemoryCache>();

        return collection;
    }
}