using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.Dto.Users;
using ITMO.Dev.ASAP.Github.Application.Mapping;
using ITMO.Dev.ASAP.Github.Domain.Users;
using MediatR;
using static ITMO.Dev.ASAP.Github.Application.Contracts.Users.Queries.FindUserById;

namespace ITMO.Dev.ASAP.Github.Application.Handlers.Users;

internal class FindUserByIdHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public FindUserByIdHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        GithubUser? user = await _context.Users.FindAsync(new object[] { request.Id }, cancellationToken);
        GithubUserDto? dto = user?.ToDto();

        return new Response(dto);
    }
}