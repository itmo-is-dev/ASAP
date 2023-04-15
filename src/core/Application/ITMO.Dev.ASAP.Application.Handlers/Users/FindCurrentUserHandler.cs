using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Users.Queries.FindCurrentUser;

namespace ITMO.Dev.ASAP.Application.Handlers.Users;

internal class FindCurrentUserHandler : IRequestHandler<Query, Response>
{
    private readonly ICurrentUser _currentUser;
    private readonly IDatabaseContext _context;

    public FindCurrentUserHandler(ICurrentUser currentUser, IDatabaseContext context)
    {
        _currentUser = currentUser;
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        User? user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id.Equals(_currentUser.Id), cancellationToken);

        UserDto? dto = user?.ToDto();

        return new Response(dto);
    }
}