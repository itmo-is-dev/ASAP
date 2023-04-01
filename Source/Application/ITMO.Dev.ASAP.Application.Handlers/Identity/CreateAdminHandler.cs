using ITMO.Dev.ASAP.Identity.Abstractions.Entities;
using ITMO.Dev.ASAP.Identity.Abstractions.Services;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Commands.CreateAdmin;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;

internal class CreateAdminHandler : IRequestHandler<Command>
{
    private readonly IIdentitySetvice _identitySetvice;

    public CreateAdminHandler(IIdentitySetvice identitySetvice)
    {
        _identitySetvice = identitySetvice;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        await _identitySetvice.CreateUserAsync(
            Guid.NewGuid(),
            request.Username,
            request.Password,
            AsapIdentityRole.AdminRoleName,
            cancellationToken);

        return Unit.Value;
    }
}