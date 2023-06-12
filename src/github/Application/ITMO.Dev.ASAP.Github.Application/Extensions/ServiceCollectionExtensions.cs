using ITMO.Dev.ASAP.Github.Application.BackgroundServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Github.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubApplication(
        this IServiceCollection collection,
        IConfiguration configuration)
    {
        if (configuration.GetSection("Github:Invites:Enabled").Get<bool>())
        {
            collection
                .AddSingleton(configuration.GetSection("Github:Invites").Get<GithubInviteBackgroundServiceConfiguration>()!)
                .AddHostedService<GithubInviteBackgroundService>();
        }

        return collection;
    }
}