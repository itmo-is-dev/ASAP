using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.Dto.PullRequests;
using ITMO.Dev.ASAP.Github.Common.Exceptions.Entities;
using ITMO.Dev.ASAP.Github.Common.Extensions;
using ITMO.Dev.ASAP.Github.Domain.Assignments;
using ITMO.Dev.ASAP.Github.Domain.SubjectCourses;
using Microsoft.EntityFrameworkCore;

namespace ITMO.Dev.ASAP.Github.Application.Specifications;

public static class AssignmentSpecifications
{
    public static IQueryable<GithubAssignment> ForBranchName(this IQueryable<GithubAssignment> queryable, string name)
        => queryable.Where(x => x.BranchName.Equals(name));

    public static IQueryable<GithubAssignment> ForSubjectCourses(
        this IQueryable<GithubAssignment> queryable,
        IQueryable<GithubSubjectCourse> subjectCourses)
    {
        return queryable.Where(ga => subjectCourses.Any(c => c.Id.Equals(ga.SubjectCourseId)));
    }

    public static async Task<GithubAssignment> GetAssignmentForPullRequestAsync(
        this IDatabaseContext context,
        PullRequestDto pullRequest,
        CancellationToken cancellationToken)
    {
        IQueryable<GithubSubjectCourse> subjectCourses = context.SubjectCourses
            .ForOrganizationName(pullRequest.Organization);

        GithubAssignment? assignment = await context.Assignments
            .ForSubjectCourses(subjectCourses)
            .ForBranchName(pullRequest.BranchName)
            .SingleOrDefaultAsync(cancellationToken);

        return assignment ?? throw EntityNotFoundException.Assignment().TaggedWithNotFound();
    }
}