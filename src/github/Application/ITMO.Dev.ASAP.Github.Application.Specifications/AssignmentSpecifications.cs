using ITMO.Dev.ASAP.Github.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.Github.Application.Dto.PullRequests;
using ITMO.Dev.ASAP.Github.Common.Exceptions.Entities;
using ITMO.Dev.ASAP.Github.Common.Extensions;
using ITMO.Dev.ASAP.Github.Domain.Assignments;

namespace ITMO.Dev.ASAP.Github.Application.Specifications;

public static class AssignmentSpecifications
{
    public static async Task<GithubAssignment> GetAssignmentForPullRequestAsync(
        this IGithubAssignmentRepository repository,
        PullRequestDto pullRequest,
        CancellationToken cancellationToken = default)
    {
        var query = GithubAssignmentQuery.Build(x => x
            .WithSubjectCourseOrganizationName(pullRequest.Organization)
            .WithBranchName(pullRequest.BranchName));

        GithubAssignment? assignment = await repository
            .QueryAsync(query, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        return assignment ?? throw EntityNotFoundException.Assignment().TaggedWithNotFound();
    }

    public static async Task<GithubAssignment?> FindAssignmentForPullRequestAsync(
        this IGithubAssignmentRepository repository,
        PullRequestDto pullRequest,
        CancellationToken cancellationToken = default)
    {
        var query = GithubAssignmentQuery.Build(x => x
            .WithSubjectCourseOrganizationName(pullRequest.Organization)
            .WithBranchName(pullRequest.BranchName));

        GithubAssignment? assignment = await repository
            .QueryAsync(query, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        return assignment;
    }
}