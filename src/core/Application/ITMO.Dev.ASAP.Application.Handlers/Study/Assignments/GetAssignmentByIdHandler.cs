using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Domain.Study.Assignments;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Assignments.Queries.GetAssignmentById;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.Assignments;

internal class GetAssignmentByIdHandler : IRequestHandler<Query, Response>
{
    private readonly IPersistenceContext _context;

    public GetAssignmentByIdHandler(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        Assignment assignment = await _context.Assignments.GetByIdAsync(request.Id, cancellationToken);
        AssignmentDto dto = assignment.ToDto();

        return new Response(dto);
    }
}