using ITMO.Dev.ASAP.Application.Dto.Submissions;
using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.Octokit.Notifications;
using ITMO.Dev.ASAP.Github.Application.Specifications;
using ITMO.Dev.ASAP.Github.Domain.Submissions;
using ITMO.Dev.ASAP.Github.Domain.Users;
using ITMO.Dev.ASAP.Presentation.Contracts.Services;
using MediatR;
using static ITMO.Dev.ASAP.Github.Application.Contracts.PullRequestEvents.PullRequestClosed;

namespace ITMO.Dev.ASAP.Github.Application.Handlers.PullRequestEvents;

#pragma warning disable IDE0072

internal class PullRequestClosedHandler : IRequestHandler<Command>
{
    private readonly IDatabaseContext _context;
    private readonly IAsapSubmissionWorkflowService _asapSubmissionWorkflowService;
    private readonly IAsapPermissionService _asapPermissionService;
    private readonly INotifierFactory _notifierFactory;

    public PullRequestClosedHandler(
        IDatabaseContext context,
        IAsapSubmissionWorkflowService asapSubmissionWorkflowService,
        IAsapPermissionService asapPermissionService,
        INotifierFactory notifierFactory)
    {
        _context = context;
        _asapSubmissionWorkflowService = asapSubmissionWorkflowService;
        _asapPermissionService = asapPermissionService;
        _notifierFactory = notifierFactory;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        GithubUser issuer = await _context.Users
            .GetForUsernameAsync(request.PullRequest.Sender, cancellationToken);

        GithubSubmission submission = await _context
            .GetSubmissionForPullRequestAsync(request.PullRequest, cancellationToken);

        bool isOrganizationMentor = await _asapPermissionService.IsSubmissionMentorAsync(
            issuer.Id,
            submission.Id,
            cancellationToken);

        SubmissionActionMessageDto result = (isOrganizationMentor, request.IsMerged) switch
        {
            (true, true) => await _asapSubmissionWorkflowService.SubmissionAcceptedAsync(
                issuer.Id,
                submission.Id,
                cancellationToken),

            (true, false) => await _asapSubmissionWorkflowService.SubmissionRejectedAsync(
                issuer.Id,
                submission.Id,
                cancellationToken),

            (false, var isMerged) => await _asapSubmissionWorkflowService.SubmissionAbandonedAsync(
                issuer.Id,
                submission.Id,
                isMerged,
                cancellationToken),
        };

        IPullRequestEventNotifier notifier = _notifierFactory.ForPullRequest(request.PullRequest);
        await notifier.SendCommentToPullRequest(result.Message);

        return Unit.Value;
    }
}