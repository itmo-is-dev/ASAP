using ITMO.Dev.ASAP.Github.Application.Octokit.Clients;
using Microsoft.Extensions.Logging;
using Octokit;
using Octokit.Webhooks;
using Octokit.Webhooks.Models;
using Repository = Octokit.Webhooks.Models.Repository;

namespace ITMO.Dev.ASAP.Github.Presentation.Webhooks.Notifiers;

public class ActionNotifier : IActionNotifier
{
    private readonly IGithubClientProvider _installationClientFactory;
    private readonly ILogger<ActionNotifier> _logger;

    public ActionNotifier(IGithubClientProvider installationClientFactory, ILogger<ActionNotifier> logger)
    {
        _installationClientFactory = installationClientFactory;
        _logger = logger;
    }

    public async Task SendComment(WebhookEvent webhookEvent, long issueNumber, string message)
    {
        ParseWebhookEvent(webhookEvent, out Repository repository, out InstallationLite installation);

        IGitHubClient installationClient = await _installationClientFactory
            .GetClientForInstallationAsync(installation.Id, default);

        _logger.LogInformation(
            "Sending comment to organization = {Organization}, repository = {Repository}, issue = {Issue}",
            repository.Owner.Login,
            repository.Name,
            issueNumber);

        await installationClient.Issue.Comment.Create(
            repository.Owner.Login,
            repository.Name,
            (int)issueNumber,
            message);
    }

    public async Task ReactInComments(WebhookEvent webhookEvent, long commentId, bool isSuccessful)
    {
        ParseWebhookEvent(webhookEvent, out Repository repository, out InstallationLite installation);

        IGitHubClient installationClient = await _installationClientFactory
            .GetClientForInstallationAsync(installation.Id, default);

        _logger.LogInformation(
            "Sending comment to organization = {Organization}, repository = {Repository}, comment id = {CommentId}",
            repository.Organization,
            repository.Name,
            commentId);

        await installationClient.Reaction.IssueComment.Create(
            repository.Id,
            (int)commentId,
            new NewReaction(isSuccessful ? ReactionType.Plus1 : ReactionType.Minus1));
    }

    private void ParseWebhookEvent(
        WebhookEvent webhookEvent,
        out Repository repository,
        out InstallationLite installation)
    {
        if (webhookEvent is null)
        {
            _logger.LogError("WebhookEvent is null");
            throw new ArgumentNullException(nameof(webhookEvent), "WebhookEvent must not be null.");
        }

        if (webhookEvent.Repository is null)
        {
            _logger.LogError("Repository to comment on is null");
            throw new ArgumentNullException(nameof(repository), "Repository to comment on must not be null.");
        }

        if (webhookEvent.Installation is null)
        {
            _logger.LogError("Installation to comment is null");
            throw new ArgumentNullException(nameof(installation), "Installation to comment on must not be null.");
        }

        repository = webhookEvent.Repository;
        installation = webhookEvent.Installation;
    }
}