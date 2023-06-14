using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Domain.Groups;
using ITMO.Dev.ASAP.Domain.Students;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Users.Commands.DismissStudentFromGroup;

namespace ITMO.Dev.ASAP.Application.Handlers.Students;

internal class DismissStudentFromGroupHandler : IRequestHandler<Command>
{
    private readonly IPersistenceContext _context;

    public DismissStudentFromGroupHandler(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        Student student = await _context.Students
            .GetByIdAsync(request.StudentId, cancellationToken);

        if (student.Group is null)
            return;

        StudentGroup group = await _context.StudentGroups.GetByIdAsync(student.Group.Id, cancellationToken);

        student.DismissFromStudyGroup(group);

        _context.Students.Update(student);
        _context.StudentGroups.Update(group);

        await _context.SaveChangesAsync(cancellationToken);
    }
}