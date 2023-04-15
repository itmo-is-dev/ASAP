using ITMO.Dev.ASAP.Github.Application.Dto.SubjectCourses;
using MediatR;

namespace ITMO.Dev.ASAP.Github.Application.Contracts.SubjectCourses.Queries;

public static class FindSubjectCoursesByIds
{
    public record Query(IEnumerable<Guid> Ids) : IRequest<Response>;

    public record Response(IReadOnlyCollection<GithubSubjectCourseDto> SubjectCourses);
}