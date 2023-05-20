using ITMO.Dev.ASAP.Github.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Github.Domain.SubjectCourses;

namespace ITMO.Dev.ASAP.Github.Application.DataAccess.Repositories;

public interface IGithubSubjectCourseRepository
{
    IAsyncEnumerable<GithubSubjectCourse> QueryAsync(
        GithubSubjectCourseQuery query,
        CancellationToken cancellationToken);

    void Add(GithubSubjectCourse subjectCourse);

    void Update(GithubSubjectCourse subjectCourse);
}