using ITMO.Dev.ASAP.Github.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;

namespace ITMO.Dev.ASAP.Github.Presentation.Services.Dummy;

internal class DummyGithubSubjectCourseService : IGithubSubjectCourseService
{
    public Task<IReadOnlyCollection<GithubSubjectCourseDto>> FindByIdsAsync(
        IEnumerable<Guid> subjectCourseIds,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<GithubSubjectCourseDto>>(Array.Empty<GithubSubjectCourseDto>());
    }
}