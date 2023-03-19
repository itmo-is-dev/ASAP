using ITMO.Dev.ASAP.Commands.Extensions;
using ITMO.Dev.ASAP.Github.Presentation.Webhooks.Notifiers;
using ITMO.Dev.ASAP.Github.Presentation.Webhooks.Processing;
using Microsoft.Extensions.DependencyInjection;
using Octokit.Webhooks;

namespace ITMO.Dev.ASAP.Github.Presentation.Webhooks.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubPresentation(this IServiceCollection collection)
    {
        collection.AddScoped<PullRequestWebhookEventProcessor>();
        collection.AddScoped<PullRequestReviewWebhookEventProcessor>();
        collection.AddScoped<IssueCommentWebhookProcessor>();

        collection.AddScoped<WebhookEventProcessor, AsapWebhookEventProcessor>();

        collection.AddScoped<IActionNotifier, ActionNotifier>();
        collection.AddPresentationCommands();

        return collection;
    }
}