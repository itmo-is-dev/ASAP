using ITMO.Dev.ASAP.Application.Abstractions.SubjectCourses;
using ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Notifications;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Application.Dto.Tables;
using MediatR;
using Microsoft.Extensions.Logging;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Notifications.SubjectCoursePointsOutdated;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourses;

internal class SubjectCoursePointsOutdatedHandler : INotificationHandler<Notification>
{
    private readonly ILogger<SubjectCoursePointsOutdatedHandler> _logger;
    private readonly ISubjectCourseService _service;
    private readonly IPublisher _publisher;

    public SubjectCoursePointsOutdatedHandler(
        ILogger<SubjectCoursePointsOutdatedHandler> logger,
        ISubjectCourseService service,
        IPublisher publisher)
    {
        _logger = logger;
        _service = service;
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
                "Error updating course points for subject course {SubjectCourseId}",
                notification.SubjectCourseId);
        }
    }

    private async Task ExecuteAsync(
        Notification notification,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Start updating for points sheet of course {SubjectCourseId}",
            notification.SubjectCourseId);

        _logger.LogInformation("Started to collecting all course {CourseId} points", notification.SubjectCourseId);

        SubjectCoursePointsDto points = await _service.CalculatePointsAsync(
            notification.SubjectCourseId, cancellationToken);

        _logger.LogInformation("Finished to collect all course {CourseId} points", notification.SubjectCourseId);

        if (_logger.IsEnabled(LogLevel.Trace))
        {
            _logger.LogTrace("Calculated points:");

            IEnumerable<(StudentPointsDto Student, AssignmentPointsDto Points, AssignmentDto Assignment)> table =
                points.StudentsPoints.SelectMany(x => x.Points, (s, a) =>
                {
                    AssignmentDto assignment = points.Assignments.Single(x => x.Id.Equals(a.AssignmentId));
                    return (Student: s, Points: a, Assignment: assignment);
                });

            foreach ((StudentPointsDto student, AssignmentPointsDto studentPoints, AssignmentDto assignment) in table)
            {
                _logger.LogTrace(
                    "\t{Student} - {Assignment}: {Points}, banned: {Banned}",
                    student.Student.GitHubUsername,
                    assignment.Title,
                    studentPoints.Points,
                    studentPoints.IsBanned);
            }
        }

        var updatedNotification = new SubjectCoursePointsUpdated.Notification(notification.SubjectCourseId, points);
        await _publisher.Publish(updatedNotification, default);
    }
}