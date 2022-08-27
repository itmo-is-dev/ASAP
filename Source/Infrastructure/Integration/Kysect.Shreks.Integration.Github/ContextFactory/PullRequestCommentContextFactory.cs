using Kysect.Shreks.Application.Abstractions.Github.Queries;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Dto.Github;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Integration.Github.ContextFactory;

public class PullRequestCommentContextFactory : ICommandContextFactory
{
    private readonly IMediator _mediator;
    private readonly ILogger _log;
    private readonly GithubPullRequestDescriptor _pullRequestDescriptor;

    public PullRequestCommentContextFactory(
        IMediator mediator,
        GithubPullRequestDescriptor pullRequestDescriptor,
        ILogger log)
    {
        _mediator = mediator;
        _pullRequestDescriptor = pullRequestDescriptor;
        _log = log;
    }

    public async Task<BaseContext> CreateBaseContext(CancellationToken cancellationToken)
    {
        var userId = await GetUserId(cancellationToken);
        
        return new BaseContext(_mediator, _log, userId);
    }

    public async Task<SubmissionContext> CreateSubmissionContext(CancellationToken cancellationToken)
    {
        var userId = await GetUserId(cancellationToken);

        var submissionQuery = new GetCurrentUnratedSubmissionByPrNumber.Query(_pullRequestDescriptor);
        var submissionResponse = await _mediator.Send(submissionQuery, cancellationToken);

        return new SubmissionContext(_mediator, _log, userId, submissionResponse.SubmissionDto);
    }

    public async Task<PullRequestContext> CreatePullRequestContext(CancellationToken cancellationToken)
    {
        var userId = await GetUserId(cancellationToken);

        return new PullRequestContext(_mediator, _log, userId, _pullRequestDescriptor);
    }

    private async Task<Guid> GetUserId(CancellationToken cancellationToken)
    {
        var query = new GetUserByGithubUsername.Query(_pullRequestDescriptor.Sender);
        var response = await _mediator.Send(query, cancellationToken);
        return response.UserId;
    }
}