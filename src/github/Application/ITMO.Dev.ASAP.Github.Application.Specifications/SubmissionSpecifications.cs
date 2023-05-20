using ITMO.Dev.ASAP.Github.Application.DataAccess.Models;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.Github.Application.Dto.PullRequests;
using ITMO.Dev.ASAP.Github.Common.Exceptions.Entities;
using ITMO.Dev.ASAP.Github.Common.Extensions;
using ITMO.Dev.ASAP.Github.Domain.Submissions;

namespace ITMO.Dev.ASAP.Github.Application.Specifications;

public static class SubmissionSpecifications
{
    public static async Task<GithubSubmission> GetSubmissionForPullRequestAsync(
        this IGithubSubmissionRepository repository,
        PullRequestDto pullRequest,
        CancellationToken cancellationToken = default)
    {
        var query = GithubSubmissionQuery.Build(x => x
            .WithRepositoryName(pullRequest.Repository)
            .WithPullRequestNumber(pullRequest.PullRequestNumber)
            .WithOrganizationName(pullRequest.Organization)
            .WithAssignmentBranchName(pullRequest.BranchName)
            .WithOrderByCreatedAt(OrderDirection.Descending));

        GithubSubmission? submission = await repository
            .QueryAsync(query, cancellationToken)
            .FirstOrDefaultAsync(cancellationToken);

        return submission ?? throw EntityNotFoundException.Submission().TaggedWithNotFound();
    }
}