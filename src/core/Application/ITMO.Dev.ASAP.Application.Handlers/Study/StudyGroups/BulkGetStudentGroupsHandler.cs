using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Domain.Groups;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.StudyGroups.Queries.BulkGetStudyGroups;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.StudyGroups;

internal class BulkGetStudentGroupsHandler : IRequestHandler<Query, Response>
{
    private readonly IPersistenceContext _context;

    public BulkGetStudentGroupsHandler(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var query = StudentGroupQuery.Build(x => x.WithIds(request.Ids));

        IAsyncEnumerable<StudentGroup> groups = _context.StudentGroups.QueryAsync(query, cancellationToken);
        StudyGroupDto[] dto = await groups.Select(x => x.ToDto()).ToArrayAsync(cancellationToken);

        return new Response(dto);
    }
}