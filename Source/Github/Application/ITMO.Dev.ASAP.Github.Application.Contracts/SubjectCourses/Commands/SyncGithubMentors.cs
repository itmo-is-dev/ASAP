using MediatR;

namespace ITMO.Dev.ASAP.Github.Application.Contracts.SubjectCourses.Commands;

internal static class SyncGithubMentors
{
    public record Command(string OrganizationName) : IRequest<Unit>;
}