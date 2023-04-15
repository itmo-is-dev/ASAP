using ITMO.Dev.ASAP.Github.Application.Octokit.Notifications;
using Microsoft.Extensions.Logging;
using Octokit.Webhooks;

namespace ITMO.Dev.ASAP.Github.Presentation.Webhooks.Notifiers;

public class PullRequestEventNotifier : IPullRequestEventNotifier
{
    private readonly IActionNotifier _actionNotifier;
    private readonly long _issueNumber;
    private readonly ILogger _logger;
    private readonly WebhookEvent _webhookEvent;

    public PullRequestEventNotifier(
        IActionNotifier actionNotifier,
        WebhookEvent webhookEvent,
        long issueNumber,
        ILogger logger)
    {
        _actionNotifier = actionNotifier;
        _webhookEvent = webhookEvent;
        _issueNumber = issueNumber;
        _logger = logger;
    }

    public async Task SendCommentToPullRequest(string message)
    {
        if (string.IsNullOrEmpty(message))
            return;

        await _actionNotifier.SendComment(_webhookEvent, _issueNumber, message);
        _logger.LogInformation("Send comment to PR: {Message}", message);
    }
}