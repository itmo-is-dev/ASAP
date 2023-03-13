using ITMO.Dev.ASAP.Application.Dto.Tables;
using ITMO.Dev.ASAP.Rpc.Abstractions;
using ITMO.Dev.ASAP.Rpc.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Queues.Notifications.SubmissionsQueueUpdated;

namespace ITMO.Dev.ASAP.Rpc.Handlers;

internal class QueueUpdateNotificationHandler : INotificationHandler<Notification>
{
    private readonly IHubContext<QueueHub, IQueueHub> _hubContext;

    public QueueUpdateNotificationHandler(IHubContext<QueueHub, IQueueHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Handle(Notification notification, CancellationToken cancellationToken)
    {
        SubmissionsQueueDto submissionsQueue = notification.SubmissionsQueue;
        await _hubContext.Clients.All.SendUpdateQueueMessage(submissionsQueue, cancellationToken);
    }
}