using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.Dto.PullRequests;
using ITMO.Dev.ASAP.Github.Common.Exceptions.Entities;
using ITMO.Dev.ASAP.Github.Common.Extensions;
using ITMO.Dev.ASAP.Github.Domain.SubjectCourses;
using ITMO.Dev.ASAP.Github.Domain.Submissions;
using Microsoft.EntityFrameworkCore;

namespace ITMO.Dev.ASAP.Github.Application.Specifications;

public static class SubmissionSpecifications
{
    public static IQueryable<GithubSubmission> ForPullRequestNumber(
        this IQueryable<GithubSubmission> queryable,
        long pullRequestNumber)
    {
        return queryable.Where(x => x.PullRequestNumber.Equals(pullRequestNumber));
    }

    public static IQueryable<GithubSubmission> ForAssignments(
        this IQueryable<GithubSubmission> queryable,
        IQueryable<Guid> assignmentIds)
    {
        return queryable.Where(x => assignmentIds.Contains(x.AssignmentId));
    }

    public static IQueryable<GithubSubmission> ForRepository(
        this IQueryable<GithubSubmission> queryable,
        string repository)
    {
        return queryable.Where(x => x.Repository.ToLower().Equals(repository.ToLower()));
    }

    public static async Task<GithubSubmission> GetSubmissionForPullRequestAsync(
        this IDatabaseContext context,
        PullRequestDto pullRequest,
        CancellationToken cancellationToken)
    {
        IQueryable<GithubSubjectCourse> subjectCourses = context.SubjectCourses
            .ForOrganizationName(pullRequest.Organization);

        IQueryable<Guid> assignments = context.Assignments
            .ForBranchName(pullRequest.BranchName)
            .ForSubjectCourses(subjectCourses)
            .Select(x => x.Id);

        GithubSubmission? submission = await context.Submissions
            .ForRepository(pullRequest.Repository)
            .ForPullRequestNumber(pullRequest.PullRequestNumber)
            .ForAssignments(assignments)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (submission is null)
            throw EntityNotFoundException.Submission().TaggedWithNotFound();

        return submission;
    }
}