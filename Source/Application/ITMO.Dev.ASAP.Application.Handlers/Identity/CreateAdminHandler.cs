using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Commands.CreateAdmin;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;

internal class CreateAdminHandler : IRequestHandler<Command>
{
    private readonly IAuthorizationService _authorizationService;

    public CreateAdminHandler(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        await _authorizationService.CreateUserAsync(
            Guid.NewGuid(),
            request.Username,
            request.Password,
            AsapIdentityRoleNames.AdminRoleName,
            cancellationToken);

        return Unit.Value;
    }
}