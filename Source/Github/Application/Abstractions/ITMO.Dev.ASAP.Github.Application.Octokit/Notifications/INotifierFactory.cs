using ITMO.Dev.ASAP.Github.Application.Dto.PullRequests;

namespace ITMO.Dev.ASAP.Github.Application.Octokit.Notifications;

public interface INotifierFactory
{
    IPullRequestEventNotifier ForPullRequest(PullRequestDto pullRequest);

    IPullRequestCommentEventNotifier ForPullRequestComment(PullRequestDto pullRequest, long commendId);
}