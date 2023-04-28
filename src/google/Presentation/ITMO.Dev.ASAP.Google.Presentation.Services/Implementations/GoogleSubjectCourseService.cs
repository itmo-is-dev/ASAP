using ITMO.Dev.ASAP.Google.Application.Contracts.SubjectCourses;
using ITMO.Dev.ASAP.Google.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Google.Presentation.Contracts.Services;
using MediatR;

namespace ITMO.Dev.ASAP.Google.Presentation.Services.Implementations;

public class GoogleSubjectCourseService : IGoogleSubjectCourseService
{
    private readonly IMediator _mediator;

    public GoogleSubjectCourseService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IEnumerable<GoogleSubjectCourseDto>> FindByIdsAsync(
        IEnumerable<Guid> subjectCourseIds,
        CancellationToken cancellationToken)
    {
        var query = new FindSubjectCoursesById.Query(subjectCourseIds);
        FindSubjectCoursesById.Response response = await _mediator.Send(query, cancellationToken);

        return response.SubjectCourses;
    }
}