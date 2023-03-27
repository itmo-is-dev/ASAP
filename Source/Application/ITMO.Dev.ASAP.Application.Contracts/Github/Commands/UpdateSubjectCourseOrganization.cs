using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Github.Commands;

internal static class UpdateSubjectCourseOrganization
{
    public record Command(Guid SubjectCourseId) : IRequest;
}