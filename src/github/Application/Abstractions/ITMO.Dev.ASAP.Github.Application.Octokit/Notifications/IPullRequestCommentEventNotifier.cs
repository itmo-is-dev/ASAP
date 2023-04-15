namespace ITMO.Dev.ASAP.Github.Application.Octokit.Notifications;

public interface IPullRequestCommentEventNotifier : IPullRequestEventNotifier
{
    Task ReactToUserComment(bool isSuccess);
}