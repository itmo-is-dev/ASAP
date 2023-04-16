using ITMO.Dev.ASAP.Github.Application.Octokit.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Octokit.Webhooks.AspNetCore;

namespace ITMO.Dev.ASAP.Github.Presentation.Webhooks.Extensions;

public static class AppBuilderExtensions
{
    public static IApplicationBuilder UseGithubIntegration(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
        {
            IOptions<GithubIntegrationConfiguration> options = endpoints.ServiceProvider
                .GetRequiredService<IOptions<GithubIntegrationConfiguration>>();

            endpoints.MapGitHubWebhooks(secret: options.Value.GithubAppConfiguration.GithubAppSecret);
        });

        return app;
    }
}