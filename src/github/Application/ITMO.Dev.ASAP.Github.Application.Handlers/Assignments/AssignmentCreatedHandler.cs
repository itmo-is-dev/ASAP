using ITMO.Dev.ASAP.Application.Contracts.Study.Assignments.Notifications;
using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Domain.Assignments;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ITMO.Dev.ASAP.Github.Application.Handlers.Assignments;

public class AssignmentCreatedHandler : INotificationHandler<AssignmentCreated.Notification>
{
    private readonly IDatabaseContext _context;

    private readonly ILogger<AssignmentCreatedHandler> _logger;

    public AssignmentCreatedHandler(IDatabaseContext context, ILogger<AssignmentCreatedHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(AssignmentCreated.Notification notification, CancellationToken cancellationToken)
    {
        var assignment = new GithubAssignment(notification.Assignment.Id, notification.Assignment.ShortName);

        if (await _context.Assignments.FindAsync(notification.Assignment.Id) is not null)
        {
            _logger.Log(
                LogLevel.Warning,
                "Updating github assignment that already exists, id: {Id}, name: {ShortName}",
                notification.Assignment.Id,
                notification.Assignment.ShortName);

            _context.Assignments.Update(assignment);
        }
        else
        {
            _context.Assignments.Add(assignment);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}