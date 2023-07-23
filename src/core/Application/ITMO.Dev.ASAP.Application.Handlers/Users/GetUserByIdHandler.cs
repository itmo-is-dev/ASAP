using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Domain.Users;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Users.Queries.GetUserById;

namespace ITMO.Dev.ASAP.Application.Handlers.Users;

internal class GetUserByIdHandler : IRequestHandler<Query, Response>
{
    private readonly IPersistenceContext _context;

    public GetUserByIdHandler(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        User user = await _context.Users.GetByIdAsync(request.UserId, cancellationToken);
        UserDto dto = user.ToDto();

        return new Response(dto);
    }
}