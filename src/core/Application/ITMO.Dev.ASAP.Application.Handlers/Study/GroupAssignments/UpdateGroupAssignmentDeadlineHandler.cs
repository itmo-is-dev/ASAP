using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Common.Exceptions;
using ITMO.Dev.ASAP.Application.Contracts.Study.GroupAssignments.Notifications;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.GroupAssignments.Commands.UpdateGroupAssignmentDeadline;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.GroupAssignments;

internal class UpdateGroupAssignmentDeadlineHandler : IRequestHandler<Command, Response>
{
    private readonly IDatabaseContext _context;
    private readonly IPublisher _publisher;
    private readonly ICurrentUser _currentUser;

    public UpdateGroupAssignmentDeadlineHandler(IDatabaseContext context, IPublisher publisher, ICurrentUser currentUser)
    {
        _context = context;
        _publisher = publisher;
        _currentUser = currentUser;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        GroupAssignment? groupAssignment = await _context
            .GroupAssignments
            .Where(groupAssignment => groupAssignment.GroupId.Equals(request.GroupId))
            .Where(groupAssignment => groupAssignment.AssignmentId.Equals(request.AssignmentId))
            .FirstOrDefaultAsync(cancellationToken);

        if (groupAssignment is null)
            throw new EntityNotFoundException("GroupAssignment not found");

        if (_currentUser.CanUpdateAllDeadlines is false)
        {
            Mentor? mentor = await _context.Mentors
                .Where(mentor => mentor.UserId.Equals(_currentUser.Id))
                .FirstOrDefaultAsync(cancellationToken);

            if (mentor?.Course.Equals(groupAssignment.Assignment.SubjectCourse) is not true)
                throw new AccessDeniedException();
        }

        groupAssignment.Deadline = request.NewDeadline;
        await _context.SaveChangesAsync(cancellationToken);

        GroupAssignmentDto dto = groupAssignment.ToDto();

        var notification = new GroupAssignmentDeadlineUpdated.Notification(dto);
        await _publisher.PublishAsync(notification, cancellationToken);

        return new Response(dto);
    }
}