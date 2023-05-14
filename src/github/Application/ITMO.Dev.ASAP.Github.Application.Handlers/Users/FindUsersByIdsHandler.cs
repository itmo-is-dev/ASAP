using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Github.Application.Dto.Users;
using ITMO.Dev.ASAP.Github.Application.Mapping;
using ITMO.Dev.ASAP.Github.Domain.Users;
using MediatR;
using static ITMO.Dev.ASAP.Github.Application.Contracts.Users.Queries.FindUsersByIds;

namespace ITMO.Dev.ASAP.Github.Application.Handlers.Users;

internal class FindUsersByIdsHandler : IRequestHandler<Query, Response>
{
    private readonly IPersistenceContext _context;

    public FindUsersByIdsHandler(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var query = GithubUserQuery.Build(x => x.WithIds(request.Ids));

        IAsyncEnumerable<GithubUser> users = _context.Users.QueryAsync(query, cancellationToken);
        List<GithubUserDto> dto = await users.Select(x => x.ToDto()).ToListAsync(cancellationToken: cancellationToken);

        return new Response(dto);
    }
}