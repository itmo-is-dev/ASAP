using MediatR;

namespace ITMO.Dev.ASAP.Github.Application.Contracts.SubjectCourses.Commands;

internal static class UpdateSubjectCourseOrganizations
{
    public record Command : IRequest;
}