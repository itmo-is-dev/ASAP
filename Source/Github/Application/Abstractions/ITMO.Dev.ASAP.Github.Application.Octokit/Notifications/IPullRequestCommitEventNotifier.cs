namespace ITMO.Dev.ASAP.Github.Application.Octokit.Notifications;

public interface IPullRequestCommitEventNotifier : IPullRequestEventNotifier
{
    Task SendCommentToTriggeredCommit(string message);
}