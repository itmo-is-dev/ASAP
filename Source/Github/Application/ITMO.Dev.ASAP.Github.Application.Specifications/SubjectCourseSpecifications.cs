using ITMO.Dev.ASAP.Github.Domain.SubjectCourses;

namespace ITMO.Dev.ASAP.Github.Application.Specifications;

public static class SubjectCourseSpecifications
{
    public static IQueryable<GithubSubjectCourse> ForOrganizationName(
        this IQueryable<GithubSubjectCourse> queryable,
        string organizationName)
    {
        return queryable.Where(x => x.OrganizationName.ToLower().Equals(organizationName.ToLower()));
    }
}