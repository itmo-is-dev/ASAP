using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourseGroups.Notifications;

public static class SubjectCourseGroupQueueOutdated
{
    public record Notification(Guid SubjectCourseId, Guid GroupId) : INotification;
}