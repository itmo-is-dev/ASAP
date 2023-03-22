using ITMO.Dev.ASAP.Github.Application.Dto.PullRequests;
using ITMO.Dev.ASAP.Github.Application.Octokit.Client;
using ITMO.Dev.ASAP.Github.Application.Octokit.Extensions;
using ITMO.Dev.ASAP.Github.Presentation.Webhooks.Notifiers;
using Microsoft.Extensions.Logging;
using Octokit;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;
using Octokit.Webhooks.Events.PullRequestReview;
using Octokit.Webhooks.Models;
using PullRequestReviewEvent = Octokit.Webhooks.Events.PullRequestReviewEvent;

namespace ITMO.Dev.ASAP.Github.Presentation.Webhooks.Processing;

public class AsapWebhookEventProcessor : WebhookEventProcessor
{
    private readonly IOrganizationGithubClientProvider _clientProvider;
    private readonly ILogger<AsapWebhookEventProcessor> _logger;
    private readonly EventNotifierProxy _notifierProxy;

    private readonly PullRequestWebhookEventProcessor _pullRequestWebhookEventProcessor;
    private readonly PullRequestReviewWebhookEventProcessor _pullRequestReviewWebhookEventProcessor;
    private readonly IssueCommentWebhookProcessor _issueCommentWebhookProcessor;

    public AsapWebhookEventProcessor(
        ILogger<AsapWebhookEventProcessor> logger,
        IOrganizationGithubClientProvider clientProvider,
        PullRequestWebhookEventProcessor pullRequestWebhookEventProcessor,
        PullRequestReviewWebhookEventProcessor pullRequestReviewWebhookEventProcessor,
        IssueCommentWebhookProcessor issueCommentWebhookProcessor,
        EventNotifierProxy notifierProxy)
    {
        _logger = logger;
        _clientProvider = clientProvider;
        _pullRequestWebhookEventProcessor = pullRequestWebhookEventProcessor;
        _pullRequestReviewWebhookEventProcessor = pullRequestReviewWebhookEventProcessor;
        _issueCommentWebhookProcessor = issueCommentWebhookProcessor;
        _notifierProxy = notifierProxy;
    }

    protected override async Task ProcessPullRequestWebhookAsync(
        WebhookHeaders headers,
        PullRequestEvent pullRequestEvent,
        PullRequestAction action)
    {
        PullRequestDto pullRequest = CreateDescriptor(pullRequestEvent);
        ILogger repositoryLogger = _logger.ToPullRequestLogger(pullRequest);

        _notifierProxy.FromPullRequestEvent(pullRequestEvent, pullRequest);

        const string methodName = nameof(ProcessPullRequestWebhookAsync);

        if (IsSenderBotOrNull(pullRequestEvent))
        {
            repositoryLogger.LogTrace($"{methodName} was skipped because sender is bot or null");
            return;
        }

        await _pullRequestWebhookEventProcessor.ProcessAsync(
            pullRequest,
            pullRequestEvent,
            action);
    }

    protected override async Task ProcessPullRequestReviewWebhookAsync(
        WebhookHeaders headers,
        PullRequestReviewEvent pullRequestReviewEvent,
        PullRequestReviewAction action)
    {
        PullRequestDto pullRequest = CreateDescriptor(pullRequestReviewEvent);
        ILogger repositoryLogger = _logger.ToPullRequestLogger(pullRequest);

        _notifierProxy.FromPullRequestReviewEvent(pullRequestReviewEvent, pullRequest);

        const string methodName = nameof(ProcessPullRequestReviewWebhookAsync);

        if (IsSenderBotOrNull(pullRequestReviewEvent))
        {
            repositoryLogger.LogTrace($"{methodName} was skipped because sender is bot or null");
            return;
        }

        await _pullRequestReviewWebhookEventProcessor.ProcessAsync(
            pullRequest,
            pullRequestReviewEvent,
            action);
    }

    protected override async Task ProcessIssueCommentWebhookAsync(
        WebhookHeaders headers,
        IssueCommentEvent issueCommentEvent,
        IssueCommentAction action)
    {
        PullRequestDto pullRequest = await GetPullRequestDescriptor(issueCommentEvent);
        ILogger repositoryLogger = _logger.ToPullRequestLogger(pullRequest);

        _notifierProxy.FromIssueCommentEvent(issueCommentEvent, pullRequest);

        const string methodName = nameof(ProcessIssueCommentWebhookAsync);

        if (IsSenderBotOrNull(issueCommentEvent))
        {
            repositoryLogger.LogTrace($"{methodName} was skipped because sender is bot or null");
            return;
        }

        if (IsPullRequestCommand(issueCommentEvent) is false)
        {
            repositoryLogger.LogTrace(
                "Skipping commit in {IssueId}. Issue comments is not supported",
                issueCommentEvent.Issue.Id);

            return;
        }

        await _issueCommentWebhookProcessor.ProcessAsync(pullRequest, issueCommentEvent, action);
    }

    private static bool IsPullRequestCommand(IssueCommentEvent issueCommentEvent)
    {
        return issueCommentEvent.Issue.PullRequest.Url is not null;
    }

    private static bool IsSenderBotOrNull(WebhookEvent webhookEvent)
    {
        return webhookEvent.Sender is null || webhookEvent.Sender.Type == UserType.Bot;
    }

    private static PullRequestDto CreateDescriptor(PullRequestReviewEvent pullRequestReviewEvent)
    {
        return new PullRequestDto(
            pullRequestReviewEvent.Sender!.Login,
            pullRequestReviewEvent.Review.HtmlUrl,
            pullRequestReviewEvent.Organization!.Login,
            pullRequestReviewEvent.Repository!.Name,
            pullRequestReviewEvent.PullRequest.Head.Ref,
            pullRequestReviewEvent.PullRequest.Number);
    }

    private PullRequestDto CreateDescriptor(PullRequestEvent @event)
    {
        string login = @event.Sender!.Login;
        string payload = @event.PullRequest.HtmlUrl;
        string organization = @event.Organization!.Login;
        string repository = @event.Repository!.Name;
        string branch = @event.PullRequest.Head.Ref;
        long prNum = @event.PullRequest.Number;

        var pullRequestDescriptor = new PullRequestDto(
            login,
            payload,
            organization,
            repository,
            branch,
            prNum);

        return pullRequestDescriptor;
    }

    private async Task<PullRequestDto> GetPullRequestDescriptor(IssueCommentEvent issueCommentEvent)
    {
        ArgumentNullException.ThrowIfNull(issueCommentEvent.Sender);
        ArgumentNullException.ThrowIfNull(issueCommentEvent.Organization);
        ArgumentNullException.ThrowIfNull(issueCommentEvent.Repository);

        GitHubClient gitHubClient = await _clientProvider.GetClient(issueCommentEvent.Organization.Login);

        PullRequest pullRequest = await gitHubClient.PullRequest
            .Get(issueCommentEvent.Repository.Id, (int)issueCommentEvent.Issue.Number);

        return new PullRequestDto(
            issueCommentEvent.Sender.Login,
            pullRequest.HtmlUrl,
            issueCommentEvent.Organization.Login,
            issueCommentEvent.Repository.Name,
            pullRequest.Head.Ref,
            pullRequest.Number);
    }
}