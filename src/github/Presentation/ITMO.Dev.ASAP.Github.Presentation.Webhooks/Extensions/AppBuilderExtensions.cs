using ITMO.Dev.ASAP.Github.Presentation.Webhooks.Configuration;
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
            IOptions<GithubWebhooksConfiguration> options = endpoints.ServiceProvider
                .GetRequiredService<IOptions<GithubWebhooksConfiguration>>();

            if (options.Value.Enabled)
            {
                endpoints.MapGitHubWebhooks(secret: options.Value.Secret);
            }
        });

        return app;
    }
}