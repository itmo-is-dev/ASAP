using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Notifications;

public static class SubjectCoursePointsOutdated
{
    public record Notification(Guid SubjectCourseId) : INotification;
}