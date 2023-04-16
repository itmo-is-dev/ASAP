using ITMO.Dev.ASAP.Application.Contracts.Study.Queues.Notifications;
using ITMO.Dev.ASAP.Presentation.Rpc.Abstractions;
using ITMO.Dev.ASAP.Presentation.Rpc.Hubs;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Queue;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ITMO.Dev.ASAP.Presentation.Rpc.Handlers;

internal class QueueUpdateNotificationHandler : INotificationHandler<QueueUpdated.Notification>
{
    private readonly IHubContext<QueueHub, IQueueHubClient> _hubContext;

    public QueueUpdateNotificationHandler(IHubContext<QueueHub, IQueueHubClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Handle(QueueUpdated.Notification notification, CancellationToken cancellationToken)
    {
        string notificationGroup = string.Concat(notification.SubjectCourseId, notification.GroupId);

        var queue = new SubjectCourseQueueModel(
            notification.SubjectCourseId,
            notification.GroupId,
            notification.SubmissionsQueue);

        IQueueHubClient group = _hubContext.Clients.Group(notificationGroup);
        await group.SendUpdateQueueMessage(queue, cancellationToken);
    }
}