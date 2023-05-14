using ITMO.Dev.ASAP.Github.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.Github.Common.Exceptions.Entities;
using ITMO.Dev.ASAP.Github.Common.Extensions;
using ITMO.Dev.ASAP.Github.Domain.SubjectCourses;

namespace ITMO.Dev.ASAP.Github.Application.Specifications;

public static class SubjectCourseSpecifications
{
    public static IAsyncEnumerable<GithubSubjectCourse> ForOrganizationName(
        this IGithubSubjectCourseRepository repository,
        string organizationName)
    {
        var query = GithubSubjectCourseQuery.Build(x => x.WithOrganizationName(organizationName));
        return repository.QueryAsync(query, default);
    }

    public static async Task<GithubSubjectCourse> GetByIdAsync(
        this IGithubSubjectCourseRepository repository,
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = GithubSubjectCourseQuery.Build(x => x.WithId(id).WithLimit(1));

        GithubSubjectCourse? subjectCourse = await repository
            .QueryAsync(query, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        return subjectCourse ?? throw EntityNotFoundException.SubjectCourse().TaggedWithNotFound();
    }
}