using ITMO.Dev.ASAP.Application.Abstractions.Queue;
using ITMO.Dev.ASAP.Application.Contracts.Study.Queues.Notifications;
using ITMO.Dev.ASAP.Application.Dto.Tables;
using MediatR;
using Microsoft.Extensions.Logging;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourseGroups.Notifications.SubjectCourseGroupQueueOutdated;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourseGroups;

internal class SubjectCourseGroupQueueOutdatedHandler : INotificationHandler<Notification>
{
    private readonly ILogger<SubjectCourseGroupQueueOutdatedHandler> _logger;
    private readonly IQueueService _queueService;
    private readonly IPublisher _publisher;

    public SubjectCourseGroupQueueOutdatedHandler(
        ILogger<SubjectCourseGroupQueueOutdatedHandler> logger,
        IQueueService queueService,
        IPublisher publisher)
    {
        _logger = logger;
        _queueService = queueService;
        _publisher = publisher;
    }

    public async Task Handle(Notification notification, CancellationToken cancellationToken)
    {
        try
        {
            await ExecuteAsync(notification, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Error while updating queue for subject course {SubjectCourseId} group {GroupId}",
                notification.SubjectCourseId,
                notification.GroupId);
        }
    }

    private async Task ExecuteAsync(Notification notification, CancellationToken cancellationToken)
    {
        SubmissionsQueueDto submissionsQueue = await _queueService.GetSubmissionsQueueAsync(
            notification.SubjectCourseId,
            notification.GroupId,
            cancellationToken);

        var updatedNotification = new QueueUpdated.Notification(
            notification.SubjectCourseId,
            notification.GroupId,
            submissionsQueue);

        await _publisher.Publish(updatedNotification, cancellationToken);
    }
}