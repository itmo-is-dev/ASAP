using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Domain.Study.GroupAssignments;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.GroupAssignments.Queries.GetGroupAssignments;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.GroupAssignments;

internal class GetGroupAssignments : IRequestHandler<Query, Response>
{
    private readonly IPersistenceContext _context;

    public GetGroupAssignments(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var query = GroupAssignmentQuery.Build(x => x.WithAssignmentId(request.AssignmentId));

        IAsyncEnumerable<GroupAssignment> assignments = _context.GroupAssignments
            .QueryAsync(query, cancellationToken);

        GroupAssignmentDto[] dto = await assignments
            .Select(x => x.ToDto())
            .ToArrayAsync(cancellationToken);

        return new Response(dto);
    }
}