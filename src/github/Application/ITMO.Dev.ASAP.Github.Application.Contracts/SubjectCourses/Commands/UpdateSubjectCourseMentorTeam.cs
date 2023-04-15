using MediatR;

namespace ITMO.Dev.ASAP.Github.Application.Contracts.SubjectCourses.Commands;

internal static class UpdateSubjectCourseMentorTeam
{
    public record Command(Guid SubjectCourseId, string MentorsTeamName) : IRequest;
}