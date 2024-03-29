using ITMO.Dev.ASAP.Application.Contracts.Study.Assignments.Notifications;
using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Domain.Study.Assignments;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses;
using ITMO.Dev.ASAP.Domain.ValueObject;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Assignments.Commands.UpdateAssignmentPoints;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.Assignments;

internal class UpdateAssignmentPointsHandler : IRequestHandler<Command, Response>
{
    private readonly IPersistenceContext _context;
    private readonly IPublisher _publisher;

    public UpdateAssignmentPointsHandler(IPersistenceContext context, IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        Assignment assignment = await _context.Assignments.GetByIdAsync(request.AssignmentId, cancellationToken);

        SubjectCourse subjectCourse = await _context.SubjectCourses
            .GetByAssignmentId(request.AssignmentId, cancellationToken);

        assignment.UpdateMinPoints(new Points(request.MinPoints));
        assignment.UpdateMaxPoints(new Points(request.MaxPoints));

        _context.Assignments.Update(assignment);
        await _context.SaveChangesAsync(cancellationToken);

        AssignmentDto dto = assignment.ToDto();

        var notification = new AssignmentPointsUpdated.Notification(dto);
        await _publisher.PublishAsync(notification, cancellationToken);

        return new Response(dto);
    }
}