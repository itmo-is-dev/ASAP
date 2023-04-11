using ITMO.Dev.ASAP.Application.Dto.Tables;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.Queues.Notifications;

public class UpdateSubmissionsQueueCache
{
    public record Notification(Guid SubjectCourseId, Guid GroupId, SubmissionsQueueDto SubmissionsQueue) : INotification;
}