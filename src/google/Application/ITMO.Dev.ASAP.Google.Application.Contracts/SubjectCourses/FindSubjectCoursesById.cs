using ITMO.Dev.ASAP.Google.Application.Dto.SubjectCourses;
using MediatR;

namespace ITMO.Dev.ASAP.Google.Application.Contracts.SubjectCourses;

internal static class FindSubjectCoursesById
{
    public record Query(IEnumerable<Guid> Ids) : IRequest<Response>;

    public record Response(IEnumerable<GoogleSubjectCourseDto> SubjectCourses);
}