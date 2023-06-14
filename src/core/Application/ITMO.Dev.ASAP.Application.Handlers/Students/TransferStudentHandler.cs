using ITMO.Dev.ASAP.Application.Contracts.Students.Notifications;
using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Domain.Groups;
using ITMO.Dev.ASAP.Domain.Students;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Students.Commands.TransferStudent;

namespace ITMO.Dev.ASAP.Application.Handlers.Students;

internal class TransferStudentHandler : IRequestHandler<Command, Response>
{
    private readonly IPersistenceContext _context;
    private readonly IPublisher _publisher;
    private readonly IGithubUserService _githubUserService;

    public TransferStudentHandler(
        IPersistenceContext context,
        IPublisher publisher,
        IGithubUserService githubUserService)
    {
        _context = context;
        _publisher = publisher;
        _githubUserService = githubUserService;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        Student student = await _context.Students.GetByIdAsync(request.StudentId, cancellationToken);
        StudentGroup group = await _context.StudentGroups.GetByIdAsync(request.GroupId, cancellationToken);

        StudentGroup? oldGroup = student.Group is null
            ? null
            : await _context.StudentGroups.GetByIdAsync(student.Group.Id, cancellationToken);

        (student, IStudentEvent evt) = student.TransferToAnotherGroup(oldGroup, group);

        await _context.Students.ApplyAsync(evt, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        StudentDto dto = await _githubUserService.MapToStudentDtoAsync(student, default);

        var notification = new StudentTransferred.Notification(
            dto.User.Id,
            request.GroupId,
            oldGroup?.Id);

        await _publisher.PublishAsync(notification, cancellationToken);

        return new Response(dto);
    }
}