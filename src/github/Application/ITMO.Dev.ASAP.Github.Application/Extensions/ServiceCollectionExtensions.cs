using ITMO.Dev.ASAP.Github.Application.BackgroundServices;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Github.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubApplication(this IServiceCollection collection)
    {
        collection.AddHostedService<GithubInviteBackgroundService>();
        return collection;
    }
}