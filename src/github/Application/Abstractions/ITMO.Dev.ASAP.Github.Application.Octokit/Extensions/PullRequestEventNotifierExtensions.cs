using ITMO.Dev.ASAP.Application.Dto.Submissions;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Github.Application.Octokit.Notifications;
using ITMO.Dev.ASAP.Github.Common.Exceptions;

namespace ITMO.Dev.ASAP.Github.Application.Octokit.Extensions;

public static class PullRequestEventNotifierExtensions
{
    public static async Task NotifySubmissionUpdate(
        this IPullRequestEventNotifier pullRequestCommitEventNotifier,
        SubmissionRateDto submission)
    {
        string message = $"Submission {submission.Code} was updated." +
                         $"\nState: {submission.State}" +
                         $"\nDate: {submission.SubmissionDate}";

        await pullRequestCommitEventNotifier.SendCommentToPullRequest(message);
    }

    public static async Task SendExceptionMessageSafe(
        this IPullRequestEventNotifier notifier,
        Exception exception)
    {
        string message = exception switch
        {
            HttpTaggedException e => e.Wrapped.Message,
            AsapGithubException e => e.Message,
            DomainException e => e.Message,
            _ => "An internal error occurred while processing command. Contact support for details.",
        };

        await notifier.SendCommentToPullRequest(message);
    }
}