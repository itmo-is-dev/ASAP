using ITMO.Dev.ASAP.Application.Contracts.Study.Assignments.Notifications;
using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Common.Exceptions.Entities;
using ITMO.Dev.ASAP.Github.Domain.Assignments;
using MediatR;

namespace ITMO.Dev.ASAP.Github.Application.Handlers.Assignments;

public class AssignmentCreatedHandler : INotificationHandler<AssignmentCreated.Notification>
{
    private readonly IDatabaseContext _context;

    public AssignmentCreatedHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task Handle(AssignmentCreated.Notification notification, CancellationToken cancellationToken)
    {
        if (await _context.Assignments.FindAsync(notification.Assignment.Id) is not null)
        {
            throw GithubAssignmentException.AssignmentAlreadyExists(notification.Assignment.Id, notification.Assignment.Title);
        }

        var assignment = new GithubAssignment(notification.Assignment.Id, notification.Assignment.ShortName);

        await _context.Assignments.AddAsync(assignment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}