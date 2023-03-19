namespace ITMO.Dev.ASAP.Github.Application.Octokit.Notifications;

public interface IPullRequestEventNotifier
{
    Task SendCommentToPullRequest(string message);
}