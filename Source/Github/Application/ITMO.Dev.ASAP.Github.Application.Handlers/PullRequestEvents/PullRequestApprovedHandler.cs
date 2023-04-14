using ITMO.Dev.ASAP.Application.Dto.Submissions;
using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.Octokit.Notifications;
using ITMO.Dev.ASAP.Github.Application.Specifications;
using ITMO.Dev.ASAP.Github.Domain.Submissions;
using ITMO.Dev.ASAP.Github.Domain.Users;
using ITMO.Dev.ASAP.Presentation.Contracts.Services;
using MediatR;
using static ITMO.Dev.ASAP.Github.Application.Contracts.PullRequestEvents.PullRequestApproved;

namespace ITMO.Dev.ASAP.Github.Application.Handlers.PullRequestEvents;

internal class PullRequestApprovedHandler : IRequestHandler<Command>
{
    private readonly IAsapSubmissionWorkflowService _submissionWorkflowService;
    private readonly IDatabaseContext _context;
    private readonly IPullRequestEventNotifier _notifier;

    public PullRequestApprovedHandler(
        IAsapSubmissionWorkflowService submissionWorkflowService,
        IDatabaseContext context,
        IPullRequestEventNotifier notifier)
    {
        _submissionWorkflowService = submissionWorkflowService;
        _context = context;
        _notifier = notifier;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        GithubUser issuer = await _context.Users
            .GetForUsernameAsync(request.PullRequest.Sender, cancellationToken);

        GithubSubmission submission = await _context
            .GetSubmissionForPullRequestAsync(request.PullRequest, cancellationToken);

        SubmissionActionMessageDto result = await _submissionWorkflowService.SubmissionApprovedAsync(
            issuer.Id,
            submission.Id,
            cancellationToken);

        await _notifier.SendCommentToPullRequest(result.Message);

        return Unit.Value;
    }
}