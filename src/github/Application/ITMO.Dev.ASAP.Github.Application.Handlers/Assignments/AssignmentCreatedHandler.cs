using ITMO.Dev.ASAP.Application.Contracts.Study.Assignments.Notifications;
using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Github.Domain.Assignments;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ITMO.Dev.ASAP.Github.Application.Handlers.Assignments;

public class AssignmentCreatedHandler : INotificationHandler<AssignmentCreated.Notification>
{
    private readonly IPersistenceContext _context;

    private readonly ILogger<AssignmentCreatedHandler> _logger;

    public AssignmentCreatedHandler(IPersistenceContext context, ILogger<AssignmentCreatedHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(AssignmentCreated.Notification notification, CancellationToken cancellationToken)
    {
        var assignment = new GithubAssignment(notification.Assignment.Id, notification.Assignment.SubjectCourseId, notification.Assignment.ShortName);

        var query = GithubAssignmentQuery.Build(x => x.WithId(notification.Assignment.Id));
        if (await _context.Assignments.QueryAsync(query, cancellationToken).FirstOrDefaultAsync(cancellationToken) is not null)
        {
            Guid assignmentId = notification.Assignment.Id;
            string assignmentShortName = notification.Assignment.ShortName;
            _logger.Log(
                LogLevel.Warning,
                "Updating github assignment that already exists, id: {AssignmentId}, name: {AssignmentShortName}",
                assignmentId,
                assignmentShortName);

            _context.Assignments.Update(assignment);
        }
        else
        {
            _context.Assignments.Add(assignment);
        }

        await _context.CommitAsync(cancellationToken);
    }
}