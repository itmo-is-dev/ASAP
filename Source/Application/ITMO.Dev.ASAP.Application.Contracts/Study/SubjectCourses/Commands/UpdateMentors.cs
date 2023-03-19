using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Commands;

public static class UpdateMentors
{
    public record Command(Guid SubjectCourseId, IReadOnlyCollection<Guid> UserIds) : IRequest;
}