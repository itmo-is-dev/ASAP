using MediatR;

namespace ITMO.Dev.ASAP.Github.Application.Contracts.SubjectCourses.Commands;

public static class UpdateSubjectCourseMentorTeam
{
    public record Command(Guid SubjectCourseId, string MentorsTeamName) : IRequest;
}