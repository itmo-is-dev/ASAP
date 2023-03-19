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
    private readonly IDatabaseContext _context;
    private readonly IAsapSubmissionWorkflowService _asapSubmissionWorkflowService;
    private readonly INotifierFactory _notifierFactory;

    public PullRequestReopenedHandler(
        IDatabaseContext context,
        IAsapSubmissionWorkflowService asapSubmissionWorkflowService,
        INotifierFactory notifierFactory)
    {
        _context = context;
        _asapSubmissionWorkflowService = asapSubmissionWorkflowService;
        _notifierFactory = notifierFactory;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        GithubUser issuer = await _context.Users
            .GetForUsernameAsync(request.PullRequest.Sender, cancellationToken);

        GithubSubmission submission = await _context
            .GetSubmissionForPullRequestAsync(request.PullRequest, cancellationToken);

        SubmissionActionMessageDto result = await _asapSubmissionWorkflowService.SubmissionReactivatedAsync(
            issuer.Id,
            submission.Id,
            cancellationToken);

        IPullRequestEventNotifier notifier = _notifierFactory.ForPullRequest(request.PullRequest);
        await notifier.SendCommentToPullRequest(result.Message);

        return Unit.Value;
    }
}