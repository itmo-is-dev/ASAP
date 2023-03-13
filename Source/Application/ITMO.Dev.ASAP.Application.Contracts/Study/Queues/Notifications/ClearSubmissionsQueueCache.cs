using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.Queues.Notifications;

internal static class ClearSubmissionsQueueCache
{
    public record Notification(Guid SubjectCourseId, Guid GroupId) : INotification;
}