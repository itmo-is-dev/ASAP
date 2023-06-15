using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Domain.Users;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Users.Commands.CreateUser;

namespace ITMO.Dev.ASAP.Application.Handlers.Users;

internal class CreateUserHandler : IRequestHandler<Command, Response>
{
    private readonly IPersistenceContext _context;

    public CreateUserHandler(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var user = new User(Guid.NewGuid(), request.FirstName, request.MiddleName, request.LastName);

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        UserDto dto = user.ToDto();

        return new Response(dto);
    }
}