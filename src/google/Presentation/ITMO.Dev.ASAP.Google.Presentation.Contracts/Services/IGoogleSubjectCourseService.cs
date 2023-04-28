using ITMO.Dev.ASAP.Google.Application.Dto.SubjectCourses;

namespace ITMO.Dev.ASAP.Google.Presentation.Contracts.Services;

public interface IGoogleSubjectCourseService
{
    Task<IEnumerable<GoogleSubjectCourseDto>> FindByIdsAsync(
        IEnumerable<Guid> subjectCourseIds,
        CancellationToken cancellationToken);
}