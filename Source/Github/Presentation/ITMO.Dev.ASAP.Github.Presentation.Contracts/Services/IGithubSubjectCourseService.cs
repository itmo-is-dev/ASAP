using ITMO.Dev.ASAP.Github.Application.Dto.SubjectCourses;

namespace ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;

public interface IGithubSubjectCourseService
{
    Task<GithubSubjectCourseDto> GetByOrganizationName(string organizationName, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<GithubSubjectCourseDto>> FindByIdsAsync(
        IEnumerable<Guid> subjectCourseIds,
        CancellationToken cancellationToken);
}