using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Subjects.Commands.UpdateSubject;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.Subjects;

internal class UpdateSubjectHandler : IRequestHandler<Command, Response>
{
    private readonly IDatabaseContext _context;

    public UpdateSubjectHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        Subject subject = await _context.Subjects.GetByIdAsync(request.Id, cancellationToken);
        subject.Title = request.NewName;

        _context.Subjects.Update(subject);
        await _context.SaveChangesAsync(cancellationToken);

        return new Response(subject.ToDto());
    }
}