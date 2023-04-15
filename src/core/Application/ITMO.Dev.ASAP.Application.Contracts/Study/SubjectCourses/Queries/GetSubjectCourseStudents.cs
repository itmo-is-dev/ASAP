using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Queries;

public static class GetSubjectCourseStudents
{
    public record Query(Guid SubjectCourseIds) : IRequest<Response>;

    public record Response(IReadOnlyCollection<Guid> StudentIds);
}