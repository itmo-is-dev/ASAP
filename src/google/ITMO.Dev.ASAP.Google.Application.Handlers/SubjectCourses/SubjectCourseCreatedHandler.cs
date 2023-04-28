using ITMO.Dev.ASAP.Google.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.Google.Application.Spreadsheets.Models;
using ITMO.Dev.ASAP.Google.Application.Spreadsheets.Services;
using ITMO.Dev.ASAP.Google.Domain.SubjectCourses;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Notifications.SubjectCourseCreated;

namespace ITMO.Dev.ASAP.Google.Application.Handlers.SubjectCourses;

public class SubjectCourseCreatedHandler : INotificationHandler<Notification>
{
    private readonly ISubjectCourseRepository _subjectCourseRepository;
    private readonly ISpreadsheetService _spreadsheetService;

    public SubjectCourseCreatedHandler(
        ISubjectCourseRepository subjectCourseRepository,
        ISpreadsheetService spreadsheetService)
    {
        _subjectCourseRepository = subjectCourseRepository;
        _spreadsheetService = spreadsheetService;
    }

    public async Task Handle(Notification notification, CancellationToken cancellationToken)
    {
        SpreadsheetCreateResult result = await _spreadsheetService
            .CreateSpreadsheetAsync(notification.SubjectCourse.Title, cancellationToken);

        var subjectCourse = new GoogleSubjectCourse(notification.SubjectCourse.Id, result.SpreadsheetId);
        await _subjectCourseRepository.SaveAsync(subjectCourse, cancellationToken);
    }
}