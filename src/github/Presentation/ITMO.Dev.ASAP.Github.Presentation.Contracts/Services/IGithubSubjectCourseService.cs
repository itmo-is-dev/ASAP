using ITMO.Dev.ASAP.Github.Application.Dto.SubjectCourses;

namespace ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;

public interface IGithubSubjectCourseService
{
    Task<IReadOnlyCollection<GithubSubjectCourseDto>> FindByIdsAsync(
        IEnumerable<Guid> subjectCourseIds,
        CancellationToken cancellationToken);
}