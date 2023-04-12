using ITMO.Dev.ASAP.Application.Contracts.Study.Queues.Notifications;
using ITMO.Dev.ASAP.Presentation.Rpc.Abstractions;
using ITMO.Dev.ASAP.Presentation.Rpc.Hubs;
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

        IQueueHubClient group = _hubContext.Clients.Group(notificationGroup);
        await group.SendUpdateQueueMessage(notification.SubmissionsQueue, cancellationToken);
    }
}