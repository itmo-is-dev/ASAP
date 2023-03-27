using ITMO.Dev.ASAP.Application.Abstractions.Google.Notifications;
using ITMO.Dev.ASAP.Application.Dto.Tables;
using ITMO.Dev.ASAP.Presentation.Rpc.Abstractions;
using ITMO.Dev.ASAP.Presentation.Rpc.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Queues.Queries.GetSubmmissionsQueue;

namespace ITMO.Dev.ASAP.Presentation.Rpc.Handlers;

internal class QueueUpdateNotificationHandler : INotificationHandler<SubjectCourseGroupQueueUpdateNotification>
{
    private readonly IHubContext<QueueHub, IQueueHubClient> _hubContext;
    private readonly IMediator _mediator;

    public QueueUpdateNotificationHandler(IHubContext<QueueHub, IQueueHubClient> hubContext, IMediator mediator)
    {
        _hubContext = hubContext;
        _mediator = mediator;
    }

    public async Task Handle(SubjectCourseGroupQueueUpdateNotification notification, CancellationToken cancellationToken)
    {
        var getSubmissionsQuery = new Query(notification.SubjectCourseId, notification.GroupId);
        Response response = await _mediator.Send(getSubmissionsQuery, cancellationToken);

        string notificationGroup = string.Concat(notification.SubjectCourseId, notification.GroupId);
        SubmissionsQueueDto submissionsQueue = response.SubmissionsQueue;

        await _hubContext
            .Clients
            .Group(notificationGroup)
            .SendUpdateQueueMessage(submissionsQueue, cancellationToken);
    }
}