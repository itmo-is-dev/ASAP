using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Github.Application.Dto.Users;
using ITMO.Dev.ASAP.Github.Application.Mapping;
using ITMO.Dev.ASAP.Github.Domain.Users;
using MediatR;
using static ITMO.Dev.ASAP.Github.Application.Contracts.Users.Queries.FindUserById;

namespace ITMO.Dev.ASAP.Github.Application.Handlers.Users;

internal class FindUserByIdHandler : IRequestHandler<Query, Response>
{
    private readonly IPersistenceContext _context;

    public FindUserByIdHandler(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var query = GithubUserQuery.Build(x => x.WithId(request.Id).WithLimit(1));

        GithubUser? user = await _context.Users
            .QueryAsync(query, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        GithubUserDto? dto = user?.ToDto();

        return new Response(dto);
    }
}