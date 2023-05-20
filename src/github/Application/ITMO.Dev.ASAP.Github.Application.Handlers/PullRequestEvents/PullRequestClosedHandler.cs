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
    private readonly IAsapSubmissionWorkflowService _asapSubmissionWorkflowService;
    private readonly IAsapPermissionService _asapPermissionService;
    private readonly IPullRequestEventNotifier _notifier;
    private readonly IPersistenceContext _context;

    public PullRequestClosedHandler(
        IAsapSubmissionWorkflowService asapSubmissionWorkflowService,
        IAsapPermissionService asapPermissionService,
        IPullRequestEventNotifier notifier,
        IPersistenceContext context)
    {
        _asapSubmissionWorkflowService = asapSubmissionWorkflowService;
        _asapPermissionService = asapPermissionService;
        _notifier = notifier;
        _context = context;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        GithubUser issuer = await _context.Users.GetForUsernameAsync(request.PullRequest.Sender, cancellationToken);

        GithubSubmission submission = await _context.Submissions
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

        await _notifier.SendCommentToPullRequest(result.Message);
    }
}