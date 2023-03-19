using ITMO.Dev.ASAP.Application.Abstractions.Google;
using ITMO.Dev.ASAP.Application.Abstractions.Google.Notifications;
using ITMO.Dev.ASAP.Application.Abstractions.Google.Sheets;
using ITMO.Dev.ASAP.Application.Dto.Tables;
using ITMO.Dev.ASAP.Application.Handlers.Extensions;
using ITMO.Dev.ASAP.Core.Queue;
using ITMO.Dev.ASAP.Core.Queue.Building;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Abstractions.Extensions;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ITMO.Dev.ASAP.Application.Handlers.Google;

internal class SubjectCourseGroupQueueUpdatedHandler : INotificationHandler<SubjectCourseGroupQueueUpdatedNotification>
{
    private readonly IDatabaseContext _context;
    private readonly ILogger<SubjectCourseGroupQueueUpdatedHandler> _logger;
    private readonly IQueryExecutor _queryExecutor;
    private readonly ISheet<SubmissionsQueueDto> _sheet;
    private readonly ISubjectCourseTableService _subjectCourseTableService;
    private readonly IGithubUserService _githubUserService;

    public SubjectCourseGroupQueueUpdatedHandler(
        IDatabaseContext context,
        IQueryExecutor queryExecutor,
        ISheet<SubmissionsQueueDto> sheet,
        ISubjectCourseTableService subjectCourseTableService,
        ILogger<SubjectCourseGroupQueueUpdatedHandler> logger,
        IGithubUserService githubUserService)
    {
        _context = context;
        _queryExecutor = queryExecutor;
        _sheet = sheet;
        _subjectCourseTableService = subjectCourseTableService;
        _logger = logger;
        _githubUserService = githubUserService;
    }

    public async Task Handle(
        SubjectCourseGroupQueueUpdatedNotification notification,
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
        SubjectCourseGroupQueueUpdatedNotification notification,
        CancellationToken cancellationToken)
    {
        StudentGroup group = await _context.StudentGroups.GetByIdAsync(notification.GroupId, cancellationToken);
        SubmissionQueue queue = new DefaultQueueBuilder(group, notification.SubjectCourseId).Build();

        IEnumerable<Submission> submissions = await queue.UpdateSubmissions(
            _context.Submissions,
            _queryExecutor,
            cancellationToken);

        IReadOnlyList<QueueSubmissionDto> submissionsDto = await _githubUserService
            .MapToQueueSubmissionDto(submissions.ToArray(), cancellationToken);

        string groupName = await _context.StudentGroups
            .Where(x => x.Id.Equals(notification.GroupId))
            .Select(x => x.Name)
            .FirstAsync(cancellationToken);

        var submissionsQueue = new SubmissionsQueueDto(groupName, submissionsDto);

        string spreadsheetId = await _subjectCourseTableService
            .GetSubjectCourseTableId(notification.SubjectCourseId, cancellationToken);

        await _sheet.UpdateAsync(spreadsheetId, submissionsQueue, cancellationToken);
    }
}