using ITMO.Dev.ASAP.Application.Dto.Tables;
using ITMO.Dev.ASAP.Presentation.Rpc.Abstractions;
using ITMO.Dev.ASAP.Presentation.Rpc.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Queues.Notifications.SubmissionsQueueUpdated;

namespace ITMO.Dev.ASAP.Presentation.Rpc.Handlers;

internal class QueueUpdateNotificationHandler : INotificationHandler<Notification>
{
    private readonly IHubContext<QueueHub, IQueueHubClient> _hubContext;

    public QueueUpdateNotificationHandler(IHubContext<QueueHub, IQueueHubClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Handle(Notification notification, CancellationToken cancellationToken)
    {
        SubmissionsQueueDto submissionsQueue = notification.SubmissionsQueue;
        await _hubContext.Clients.All.SendUpdateQueueMessage(submissionsQueue, cancellationToken);
    }
}