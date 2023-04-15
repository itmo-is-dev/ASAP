using ITMO.Dev.ASAP.Github.Application.Contracts.PullRequestEvents;
using ITMO.Dev.ASAP.Github.Application.Dto.PullRequests;
using ITMO.Dev.ASAP.Github.Application.Octokit.Extensions;
using ITMO.Dev.ASAP.Github.Application.Octokit.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.PullRequest;

namespace ITMO.Dev.ASAP.Github.Presentation.Webhooks.Processing;

public class PullRequestWebhookEventProcessor
{
    private readonly ILogger<PullRequestWebhookEventProcessor> _logger;
    private readonly IMediator _mediator;
    private readonly IPullRequestEventNotifier _notifier;

    public PullRequestWebhookEventProcessor(
        ILogger<PullRequestWebhookEventProcessor> logger,
        IMediator mediator,
        IPullRequestEventNotifier notifier)
    {
        _logger = logger;
        _mediator = mediator;
        _notifier = notifier;
    }

    public async Task ProcessAsync(PullRequestDto pullRequest, PullRequestEvent pullRequestEvent, string action)
    {
        ILogger logger = _logger.ToPullRequestLogger(pullRequest);

        const string processorName = nameof(PullRequestWebhookEventProcessor);

        logger.LogInformation(
            "{MethodName}: {EventName} with type {Action}",
            processorName,
            pullRequestEvent.GetType().Name,
            action);

        try
        {
            IRequest command;

            switch (action)
            {
                case PullRequestActionValue.Synchronize:
                case PullRequestActionValue.Opened:
                    command = new PullRequestUpdated.Command(pullRequest);
                    await _mediator.Send(command);

                    break;

                case PullRequestActionValue.Reopened:
                    command = new PullRequestReopened.Command(pullRequest);
                    await _mediator.Send(command);

                    break;

                case PullRequestActionValue.Closed:
                    bool merged = pullRequestEvent.PullRequest.Merged ?? false;

                    command = new PullRequestClosed.Command(pullRequest, merged);
                    await _mediator.Send(command);

                    break;

                case PullRequestActionValue.Assigned:
                case PullRequestActionValue.ReviewRequestRemoved:
                case PullRequestActionValue.ReviewRequested:
                    logger.LogDebug("Skip pull request action with type {Action}", action);
                    break;

                default:
                    logger.LogWarning("Unsupported pull request webhook type was received: {Action}", action);
                    break;
            }
        }
        catch (Exception e)
        {
            string message = $"Failed to handle {action}";
            logger.LogError(e, "{MethodName}: {Message}", processorName, message);

            await _notifier.SendExceptionMessageSafe(e);
        }
    }
}