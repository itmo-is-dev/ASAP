using MediatR;

namespace ITMO.Dev.ASAP.Github.Application.Contracts.SubjectCourses.Commands;

internal static class UpdateSubjectCourseOrganization
{
    public record Command(Guid SubjectCourseId) : IRequest;
}