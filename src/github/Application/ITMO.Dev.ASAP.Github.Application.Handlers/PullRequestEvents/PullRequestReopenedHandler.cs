using ITMO.Dev.ASAP.Application.Dto.Submissions;
using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.Octokit.Notifications;
using ITMO.Dev.ASAP.Github.Application.Specifications;
using ITMO.Dev.ASAP.Github.Domain.Submissions;
using ITMO.Dev.ASAP.Github.Domain.Users;
using ITMO.Dev.ASAP.Presentation.Contracts.Services;
using MediatR;
using static ITMO.Dev.ASAP.Github.Application.Contracts.PullRequestEvents.PullRequestReopened;

namespace ITMO.Dev.ASAP.Github.Application.Handlers.PullRequestEvents;

internal class PullRequestReopenedHandler : IRequestHandler<Command>
{
    private readonly IAsapSubmissionWorkflowService _asapSubmissionWorkflowService;
    private readonly IPullRequestEventNotifier _notifier;
    private readonly IPersistenceContext _context;

    public PullRequestReopenedHandler(
        IAsapSubmissionWorkflowService asapSubmissionWorkflowService,
        IPullRequestEventNotifier notifier,
        IPersistenceContext context)
    {
        _asapSubmissionWorkflowService = asapSubmissionWorkflowService;
        _notifier = notifier;
        _context = context;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        GithubUser issuer = await _context.Users
            .GetForUsernameAsync(request.PullRequest.Sender, cancellationToken);

        GithubSubmission submission = await _context.Submissions
            .GetSubmissionForPullRequestAsync(request.PullRequest, cancellationToken);

        SubmissionActionMessageDto result = await _asapSubmissionWorkflowService.SubmissionReactivatedAsync(
            issuer.Id,
            submission.Id,
            cancellationToken);

        await _notifier.SendCommentToPullRequest(result.Message);
    }
}