using ITMO.Dev.ASAP.Application.Dto.Submissions;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Github.Application.Octokit.Notifications;

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
        this IPullRequestEventNotifier pullRequestEventNotifier,
        Exception exception)
    {
        if (exception is DomainException or ApplicationException)
        {
            await pullRequestEventNotifier.SendCommentToPullRequest(exception.Message);
        }
        else
        {
            const string newMessage = "An internal error occurred while processing command. Contact support for details.";
            await pullRequestEventNotifier.SendCommentToPullRequest(newMessage);
        }
    }
}