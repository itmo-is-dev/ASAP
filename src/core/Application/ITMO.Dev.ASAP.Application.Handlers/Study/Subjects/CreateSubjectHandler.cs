using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Subjects.Commands.CreateSubject;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.Subjects;

internal class CreateSubjectHandler : IRequestHandler<Command, Response>
{
    private readonly IPersistenceContext _context;

    public CreateSubjectHandler(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var subject = new Subject(Guid.NewGuid(), request.Title);

        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync(cancellationToken);

        return new Response(subject.ToDto());
    }
}