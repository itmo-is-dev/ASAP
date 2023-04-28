using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Google.Application.Abstractions;
using ITMO.Dev.ASAP.Google.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.Google.Domain.SubjectCourses;
using MediatR;
using Microsoft.Extensions.Logging;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Notifications.SubjectCoursePointsUpdated;

namespace ITMO.Dev.ASAP.Google.Application.Handlers.SubjectCourses;

public class SubjectCoursePointsUpdatedHandler : INotificationHandler<Notification>
{
    private readonly ISubjectCourseRepository _subjectCourseRepository;
    private readonly ILogger<SubjectCoursePointsUpdatedHandler> _logger;
    private readonly ISheet<SubjectCoursePointsDto> _sheet;

    public SubjectCoursePointsUpdatedHandler(
        ISubjectCourseRepository subjectCourseRepository,
        ILogger<SubjectCoursePointsUpdatedHandler> logger,
        ISheet<SubjectCoursePointsDto> sheet)
    {
        _subjectCourseRepository = subjectCourseRepository;
        _logger = logger;
        _sheet = sheet;
    }

    public async Task Handle(Notification notification, CancellationToken cancellationToken)
    {
        GoogleSubjectCourse? subjectCourse = await _subjectCourseRepository
            .FindByIdAsync(notification.SubjectCourseId, cancellationToken);

        if (subjectCourse is null)
        {
            _logger.LogWarning(
                "Tried to update google points for subject course = {SubjectCourseId}, but no spreadsheet found",
                notification.SubjectCourseId);

            return;
        }

        await _sheet.UpdateAsync(subjectCourse.SpreadsheetId, notification.Points, cancellationToken);

        _logger.LogInformation(
            "Successfully updated points sheet of course {SubjectCourseId}",
            notification.SubjectCourseId);
    }
}