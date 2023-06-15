using ITMO.Dev.ASAP.Application.Contracts.Study.Assignments.Notifications;
using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Domain.Study.Assignments;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses.Events;
using ITMO.Dev.ASAP.Domain.ValueObject;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Assignments.Commands.CreateAssignment;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.Assignments;

internal class CreateAssignmentHandler : IRequestHandler<Command, Response>
{
    private readonly IPersistenceContext _context;
    private readonly IPublisher _publisher;

    public CreateAssignmentHandler(IPersistenceContext context, IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourses
            .GetByIdAsync(request.SubjectCourseId, cancellationToken);

        var assignmentBuilder = new Assignment(
            Guid.NewGuid(),
            request.Title,
            request.ShortName,
            request.Order,
            new Points(request.MinPoints),
            new Points(request.MaxPoints));

        (ISubjectCourseEvent evt, Assignment assignment) = subjectCourse.AddAssignment(assignmentBuilder);

        await _context.SubjectCourses.ApplyAsync(evt, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        AssignmentDto dto = assignment.ToDto(subjectCourse.Id);

        var notification = new AssignmentCreated.Notification(dto);
        await _publisher.PublishAsync(notification, cancellationToken);

        return new Response(dto);
    }
}