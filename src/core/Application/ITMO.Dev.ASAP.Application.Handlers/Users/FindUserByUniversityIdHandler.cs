using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Domain.Users;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Users.Queries.FindUserByUniversityId;

namespace ITMO.Dev.ASAP.Application.Handlers.Users;

internal class FindUserByUniversityIdHandler : IRequestHandler<Query, Response>
{
    private readonly IPersistenceContext _context;

    public FindUserByUniversityIdHandler(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        User? user = await _context.Users
            .QueryAsync(UserQuery.Build(x => x.WithUniversityId(request.UniversityId)), cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        return new Response(user?.ToDto());
    }
}