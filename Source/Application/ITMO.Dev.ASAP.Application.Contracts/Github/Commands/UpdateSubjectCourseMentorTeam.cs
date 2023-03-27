using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Github.Commands;

internal static class UpdateSubjectCourseMentorTeam
{
    public record Command(Guid SubjectCourseId, string MentorsTeamName) : IRequest;
}