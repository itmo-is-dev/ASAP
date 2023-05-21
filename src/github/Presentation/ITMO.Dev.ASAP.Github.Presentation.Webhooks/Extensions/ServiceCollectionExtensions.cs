using ITMO.Dev.ASAP.Commands.Extensions;
using ITMO.Dev.ASAP.Github.Application.Octokit.Notifications;
using ITMO.Dev.ASAP.Github.Presentation.Controllers.Extensions;
using ITMO.Dev.ASAP.Github.Presentation.Webhooks.Configuration;
using ITMO.Dev.ASAP.Github.Presentation.Webhooks.Notifiers;
using ITMO.Dev.ASAP.Github.Presentation.Webhooks.Processing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Octokit.Webhooks;

namespace ITMO.Dev.ASAP.Github.Presentation.Webhooks.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubPresentation(
        this IServiceCollection collection,
        IConfiguration configuration)
    {
        if (configuration.GetSection("Github:Webhooks:Enabled").Get<bool>() is false)
            return collection;

        IConfigurationSection section = configuration.GetSection("Github:Webhooks");
        collection.Configure<GithubWebhooksConfiguration>(section);

        collection.AddScoped<EventNotifierProxy>();
        collection.AddScoped<IPullRequestEventNotifier>(x => x.GetRequiredService<EventNotifierProxy>());
        collection.AddScoped<IPullRequestCommentEventNotifier>(x => x.GetRequiredService<EventNotifierProxy>());

        collection.AddScoped<PullRequestWebhookEventProcessor>();
        collection.AddScoped<PullRequestReviewWebhookEventProcessor>();
        collection.AddScoped<IssueCommentWebhookProcessor>();

        collection.AddScoped<WebhookEventProcessor, AsapWebhookEventProcessor>();

        collection.AddScoped<IActionNotifier, ActionNotifier>();
        collection.AddPresentationCommands();

        collection.AddGithubPresentationControllers();

        return collection;
    }
}