using ITMO.Dev.ASAP.Google.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Google.Presentation.Contracts.Services;

namespace ITMO.Dev.ASAP.Google.Presentation.Services.Dummy;

public class DummyGoogleSubjectCourseService : IGoogleSubjectCourseService
{
    public Task<IEnumerable<GoogleSubjectCourseDto>> FindByIdsAsync(
        IEnumerable<Guid> subjectCourseIds,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(Enumerable.Empty<GoogleSubjectCourseDto>());
    }
}