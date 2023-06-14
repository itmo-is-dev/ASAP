using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Domain.Users;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Users.Queries.FindCurrentUser;

namespace ITMO.Dev.ASAP.Application.Handlers.Users;

internal class FindCurrentUserHandler : IRequestHandler<Query, Response>
{
    private readonly ICurrentUser _currentUser;
    private readonly IPersistenceContext _context;

    public FindCurrentUserHandler(ICurrentUser currentUser, IPersistenceContext context)
    {
        _currentUser = currentUser;
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        User? user = await _context.Users.FindByIdAsync(_currentUser.Id, cancellationToken);
        UserDto? dto = user?.ToDto();

        return new Response(dto);
    }
}