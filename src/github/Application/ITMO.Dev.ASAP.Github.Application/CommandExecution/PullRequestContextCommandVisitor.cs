using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.Submissions;
using ITMO.Dev.ASAP.Commands.CommandVisitors;
using ITMO.Dev.ASAP.Commands.SubmissionCommands;
using ITMO.Dev.ASAP.Common.Resources;
using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.Dto.PullRequests;
using ITMO.Dev.ASAP.Github.Application.Octokit.Notifications;
using ITMO.Dev.ASAP.Github.Application.Specifications;
using ITMO.Dev.ASAP.Github.Domain.Assignments;
using ITMO.Dev.ASAP.Github.Domain.Submissions;
using ITMO.Dev.ASAP.Github.Domain.Users;
using ITMO.Dev.ASAP.Presentation.Contracts.Services;

namespace ITMO.Dev.ASAP.Github.Application.CommandExecution;

public class PullRequestContextCommandVisitor : ISubmissionCommandVisitor
{
    private readonly IAsapSubmissionService _asapSubmissionService;
    private readonly IDatabaseContext _context;
    private readonly PullRequestDto _pullRequest;
    private readonly IPullRequestEventNotifier _eventNotifier;

    public PullRequestContextCommandVisitor(
        IAsapSubmissionService asapSubmissionService,
        IDatabaseContext context,
        PullRequestDto pullRequest,
        IPullRequestEventNotifier eventNotifier)
    {
        _asapSubmissionService = asapSubmissionService;
        _context = context;
        _pullRequest = pullRequest;
        _eventNotifier = eventNotifier;
    }

    public async Task VisitAsync(ActivateCommand command)
    {
        GithubUser issuer = await _context.Users.GetForUsernameAsync(_pullRequest.Sender, default);
        GithubSubmission submission = await _context.GetSubmissionForPullRequestAsync(_pullRequest, default);

        await _asapSubmissionService.ActivateSubmissionAsync(issuer.Id, submission.Id, default);

        string message = UserCommandProcessingMessage.SubmissionActivatedSuccessfully();
        await _eventNotifier.SendCommentToPullRequest(message);
    }

    public async Task VisitAsync(BanCommand command)
    {
        GithubUser issuer = await _context.Users.GetForUsernameAsync(_pullRequest.Sender, default);
        GithubUser student = await _context.Users.GetForUsernameAsync(_pullRequest.Repository, default);
        GithubAssignment assignment = await _context.GetAssignmentForPullRequestAsync(_pullRequest, default);

        SubmissionDto submission = await _asapSubmissionService.BanSubmissionAsync(
            issuer.Id,
            student.Id,
            assignment.Id,
            default,
            default);

        string message = UserCommandProcessingMessage.SubmissionBanned(submission.Id);
        await _eventNotifier.SendCommentToPullRequest(message);
    }

    public async Task VisitAsync(CreateSubmissionCommand command)
    {
        GithubUser issuer = await _context.Users.GetForUsernameAsync(_pullRequest.Sender, default);
        GithubUser user = await _context.Users.GetForUsernameAsync(_pullRequest.Repository, default);
        GithubAssignment assignment = await _context.GetAssignmentForPullRequestAsync(_pullRequest, default);

        SubmissionDto submission = await _asapSubmissionService.CreateSubmissionAsync(
            issuer.Id,
            user.Id,
            assignment.Id,
            _pullRequest.Payload,
            default);

        var githubSubmission = new GithubSubmission(
            submission.Id,
            assignment.Id,
            user.Id,
            submission.SubmissionDate,
            _pullRequest.Organization,
            _pullRequest.Repository,
            _pullRequest.PullRequestNumber);

        _context.Submissions.Add(githubSubmission);
        await _context.SaveChangesAsync(default);

        string message = UserCommandProcessingMessage.SubmissionCreated(submission.ToDisplayString());
        await _eventNotifier.SendCommentToPullRequest(message);
    }

    public async Task VisitAsync(DeactivateCommand command)
    {
        GithubUser issuer = await _context.Users.GetForUsernameAsync(_pullRequest.Sender, default);
        GithubSubmission submission = await _context.GetSubmissionForPullRequestAsync(_pullRequest, default);

        await _asapSubmissionService.DeactivateSubmissionAsync(issuer.Id, submission.Id, default);

        string message = UserCommandProcessingMessage.SubmissionDeactivatedSuccessfully();
        await _eventNotifier.SendCommentToPullRequest(message);
    }

    public async Task VisitAsync(DeleteCommand command)
    {
        GithubUser issuer = await _context.Users.GetForUsernameAsync(_pullRequest.Sender, default);
        GithubSubmission submission = await _context.GetSubmissionForPullRequestAsync(_pullRequest, default);

        await _asapSubmissionService.DeleteSubmissionAsync(
            issuer.Id,
            submission.Id,
            default);

        string message = UserCommandProcessingMessage.SubmissionDeletedSuccessfully();
        await _eventNotifier.SendCommentToPullRequest(message);
    }

    public async Task VisitAsync(HelpCommand command)
    {
        await _eventNotifier.SendCommentToPullRequest(HelpCommand.HelpString);
    }

    public async Task VisitAsync(MarkReviewedCommand command)
    {
        GithubUser issuer = await _context.Users.GetForUsernameAsync(_pullRequest.Sender, default);
        GithubSubmission submission = await _context.GetSubmissionForPullRequestAsync(_pullRequest, default);

        await _asapSubmissionService.MarkSubmissionAsReviewedAsync(
            issuer.Id,
            submission.Id,
            default);

        string message = UserCommandProcessingMessage.SubmissionMarkedAsReviewed();
        await _eventNotifier.SendCommentToPullRequest(message);
    }

    public async Task VisitAsync(RateCommand command)
    {
        GithubUser issuer = await _context.Users.GetForUsernameAsync(_pullRequest.Sender, default);
        GithubSubmission submission = await _context.GetSubmissionForPullRequestAsync(_pullRequest, default);

        SubmissionRateDto submissionDto = await _asapSubmissionService.RateSubmissionAsync(
            issuer.Id,
            submission.Id,
            command.RatingPercent,
            command.ExtraPoints,
            default);

        string message = UserCommandProcessingMessage.SubmissionRated(submissionDto.ToDisplayString());
        await _eventNotifier.SendCommentToPullRequest(message);
    }

    public async Task VisitAsync(UpdateCommand command)
    {
        GithubUser issuer = await _context.Users.GetForUsernameAsync(_pullRequest.Sender, default);
        GithubUser user = await _context.Users.GetForUsernameAsync(_pullRequest.Repository, default);
        GithubAssignment assignment = await _context.GetAssignmentForPullRequestAsync(_pullRequest, default);

        SubmissionRateDto submission = await _asapSubmissionService.UpdateSubmissionAsync(
            issuer.Id,
            user.Id,
            assignment.Id,
            command.SubmissionCode,
            command.GetDate(),
            command.RatingPercent,
            command.ExtraPoints,
            default);

        string message = UserCommandProcessingMessage.SubmissionUpdated(submission.ToDisplayString());
        await _eventNotifier.SendCommentToPullRequest(message);
    }
}