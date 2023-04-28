using ITMO.Dev.ASAP.Application.Dto.Tables;
using ITMO.Dev.ASAP.Google.Application.Abstractions;
using ITMO.Dev.ASAP.Google.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.Google.Domain.SubjectCourses;
using MediatR;
using Microsoft.Extensions.Logging;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Queues.Notifications.QueueUpdated;

namespace ITMO.Dev.ASAP.Google.Application.Handlers.Queues;

public class QueueUpdatedHandler : INotificationHandler<Notification>
{
    private readonly ILogger<QueueUpdatedHandler> _logger;
    private readonly ISubjectCourseRepository _subjectCourseRepository;
    private readonly ISheet<SubmissionsQueueDto> _sheet;

    public QueueUpdatedHandler(
        ILogger<QueueUpdatedHandler> logger,
        ISubjectCourseRepository subjectCourseRepository,
        ISheet<SubmissionsQueueDto> sheet)
    {
        _logger = logger;
        _subjectCourseRepository = subjectCourseRepository;
        _sheet = sheet;
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
        GoogleSubjectCourse? subjectCourse = await _subjectCourseRepository
            .FindByIdAsync(notification.SubjectCourseId, cancellationToken);

        if (subjectCourse is null)
        {
            _logger.LogWarning(
                "Tried to update google queue for subject course = {SubjectCourseId}, but no spreadsheet found",
                notification.SubjectCourseId);

            return;
        }

        await _sheet.UpdateAsync(subjectCourse.SpreadsheetId, notification.SubmissionsQueue, cancellationToken);
    }
}