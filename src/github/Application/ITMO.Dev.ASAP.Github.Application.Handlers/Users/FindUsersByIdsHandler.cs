using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.Dto.Users;
using ITMO.Dev.ASAP.Github.Application.Mapping;
using ITMO.Dev.ASAP.Github.Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Github.Application.Contracts.Users.Queries.FindUsersByIds;

namespace ITMO.Dev.ASAP.Github.Application.Handlers.Users;

internal class FindUsersByIdsHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public FindUsersByIdsHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<GithubUser> users = await _context.Users
            .Where(x => request.Ids.Contains(x.Id))
            .ToListAsync(cancellationToken);

        GithubUserDto[] dto = users.Select(x => x.ToDto()).ToArray();

        return new Response(dto);
    }
}