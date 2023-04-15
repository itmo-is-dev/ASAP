using ITMO.Dev.ASAP.Github.Application.Octokit.Notifications;
using Microsoft.Extensions.Logging;
using Octokit.Webhooks;

namespace ITMO.Dev.ASAP.Github.Presentation.Webhooks.Notifiers;

public class PullRequestCommentEventNotifier : PullRequestEventNotifier, IPullRequestCommentEventNotifier
{
    private readonly IActionNotifier _actionNotifier;

    private readonly long _commentId;

    private readonly ILogger _logger;
    private readonly WebhookEvent _webhookEvent;

    public PullRequestCommentEventNotifier(
        IActionNotifier actionNotifier,
        WebhookEvent webhookEvent,
        long commentId,
        long issueNumber,
        ILogger logger)
        : base(actionNotifier, webhookEvent, issueNumber, logger)
    {
        _actionNotifier = actionNotifier;
        _webhookEvent = webhookEvent;
        _commentId = commentId;
        _logger = logger;
    }

    public async Task ReactToUserComment(bool isSuccess)
    {
        await _actionNotifier.ReactInComments(
            _webhookEvent,
            _commentId,
            isSuccess);
        _logger.LogInformation("Send reaction: {IsSuccess}", isSuccess);
    }
}