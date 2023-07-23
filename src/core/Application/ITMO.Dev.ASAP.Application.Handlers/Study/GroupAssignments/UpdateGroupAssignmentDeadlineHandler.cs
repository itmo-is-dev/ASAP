using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Common.Exceptions;
using ITMO.Dev.ASAP.Application.Contracts.Study.GroupAssignments.Notifications;
using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Domain.Study.GroupAssignments;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.GroupAssignments.Commands.UpdateGroupAssignmentDeadline;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.GroupAssignments;

internal class UpdateGroupAssignmentDeadlineHandler : IRequestHandler<Command, Response>
{
    private readonly IPersistenceContext _context;
    private readonly IPublisher _publisher;
    private readonly ICurrentUser _currentUser;

    public UpdateGroupAssignmentDeadlineHandler(
        IPersistenceContext context,
        IPublisher publisher,
        ICurrentUser currentUser)
    {
        _context = context;
        _publisher = publisher;
        _currentUser = currentUser;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        GroupAssignment groupAssignment = await _context.GroupAssignments
            .GetByIdsAsync(request.GroupId, request.AssignmentId, cancellationToken);

        SubjectCourse subjectCourse = await _context.SubjectCourses
            .GetByAssignmentId(groupAssignment.Id.AssignmentId, cancellationToken);

        if (_currentUser.CanUpdateAllDeadlines is false)
        {
            var mentorQuery = MentorQuery.Build(x => x
                .WithUserId(_currentUser.Id)
                .WithSubjectCourseId(subjectCourse.Id));

            bool isMentor = await _context.Mentors
                .QueryAsync(mentorQuery, cancellationToken)
                .AnyAsync(cancellationToken);

            if (isMentor is false)
                throw new AccessDeniedException();
        }

        groupAssignment.Deadline = request.NewDeadline;

        _context.GroupAssignments.Update(groupAssignment);
        await _context.SaveChangesAsync(cancellationToken);

        GroupAssignmentDto dto = groupAssignment.ToDto();

        var notification = new GroupAssignmentDeadlineUpdated.Notification(dto);
        await _publisher.PublishAsync(notification, cancellationToken);

        return new Response(dto);
    }
}