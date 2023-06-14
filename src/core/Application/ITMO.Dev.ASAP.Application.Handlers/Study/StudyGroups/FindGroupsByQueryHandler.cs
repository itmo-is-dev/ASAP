using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Queries;
using ITMO.Dev.ASAP.Domain.Groups;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.StudyGroups.Queries.FindGroupsByQuery;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.StudyGroups;

internal class FindGroupsByQueryHandler : IRequestHandler<Query, Response>
{
    private readonly IPersistenceContext _context;
    private readonly IEntityQuery<StudentGroupQuery.Builder, GroupQueryParameter> _query;

    public FindGroupsByQueryHandler(
        IEntityQuery<StudentGroupQuery.Builder, GroupQueryParameter> query,
        IPersistenceContext context)
    {
        _query = query;
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var queryBuilder = new StudentGroupQuery.Builder();
        queryBuilder = _query.Apply(queryBuilder, request.Configuration);

        StudentGroupQuery query = queryBuilder.Build();

        IAsyncEnumerable<StudentGroup> groups = _context.StudentGroups.QueryAsync(query, cancellationToken);
        StudyGroupDto[] dto = await groups.Select(x => x.ToDto()).ToArrayAsync(cancellationToken);

        return new Response(dto);
    }
}