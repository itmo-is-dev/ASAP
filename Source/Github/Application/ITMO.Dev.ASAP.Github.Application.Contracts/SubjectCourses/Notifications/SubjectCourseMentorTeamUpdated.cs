using MediatR;

namespace ITMO.Dev.ASAP.Github.Application.Contracts.SubjectCourses.Notifications;

internal static class SubjectCourseMentorTeamUpdated
{
    public record Notification(Guid SubjectCourseId) : INotification;
}