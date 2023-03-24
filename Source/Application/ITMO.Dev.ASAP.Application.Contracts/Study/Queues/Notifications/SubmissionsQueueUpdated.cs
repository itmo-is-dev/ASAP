using ITMO.Dev.ASAP.Application.Dto.Tables;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.Queues.Notifications;

public static class SubmissionsQueueUpdated
{
    public record Notification(SubmissionsQueueDto SubmissionsQueue) : INotification;
}