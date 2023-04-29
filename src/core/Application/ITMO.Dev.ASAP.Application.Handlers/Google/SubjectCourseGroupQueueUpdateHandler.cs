using ITMO.Dev.ASAP.Application.Abstractions.Google;
using ITMO.Dev.ASAP.Application.Abstractions.Google.Notifications;
using ITMO.Dev.ASAP.Application.Abstractions.Google.Sheets;
using ITMO.Dev.ASAP.Application.Abstractions.Queue;
using ITMO.Dev.ASAP.Application.Contracts.Study.Queues.Notifications;
using ITMO.Dev.ASAP.Application.Dto.Tables;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ITMO.Dev.ASAP.Application.Handlers.Google;

internal class SubjectCourseGroupQueueUpdateHandler : INotificationHandler<SubjectCourseGroupQueueUpdateNotification>
{
    private readonly ILogger<SubjectCourseGroupQueueUpdateHandler> _logger;
    private readonly IQueueUpdateService _queueUpdateService;
    private readonly ISheet<SubmissionsQueueDto> _sheet;
    private readonly ISubjectCourseTableService _subjectCourseTableService;
    private readonly IPublisher _publisher;

    public SubjectCourseGroupQueueUpdateHandler(
        ILogger<SubjectCourseGroupQueueUpdateHandler> logger,
        IQueueUpdateService queueUpdateService,
        ISheet<SubmissionsQueueDto> sheet,
        ISubjectCourseTableService subjectCourseTableService,
        IPublisher publisher)
    {
        _logger = logger;
        _queueUpdateService = queueUpdateService;
        _sheet = sheet;
        _subjectCourseTableService = subjectCourseTableService;
        _publisher = publisher;
    }

    public async Task Handle(
        SubjectCourseGroupQueueUpdateNotification notification,
        CancellationToken cancellationToken)
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

    private async Task ExecuteAsync(
        SubjectCourseGroupQueueUpdateNotification notification,
        CancellationToken cancellationToken)
    {
        SubmissionsQueueDto submissionsQueue = await _queueUpdateService.GetSubmissionsQueueAsync(
            notification.SubjectCourseId,
            notification.GroupId,
            cancellationToken);

        string spreadsheetId = await _subjectCourseTableService
            .GetSubjectCourseTableId(notification.SubjectCourseId, cancellationToken);

        await _sheet.UpdateAsync(spreadsheetId, submissionsQueue, cancellationToken);

        var updatedNotification = new QueueUpdated.Notification(
            notification.SubjectCourseId,
            notification.GroupId,
            submissionsQueue);

        await _publisher.Publish(updatedNotification, cancellationToken);
    }
}