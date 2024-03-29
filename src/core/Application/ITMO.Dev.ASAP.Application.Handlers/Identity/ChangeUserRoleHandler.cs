using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Common.Exceptions;
using ITMO.Dev.ASAP.Application.Dto.Identity;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Commands.ChangeUserRole;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;

internal class ChangeUserRoleHandler : IRequestHandler<Command>
{
    private readonly ICurrentUser _currentUser;
    private readonly IAuthorizationService _authorizationService;

    public ChangeUserRoleHandler(ICurrentUser currentUser, IAuthorizationService authorizationService)
    {
        _currentUser = currentUser;
        _authorizationService = authorizationService;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        IdentityUserDto user = await _authorizationService.GetUserByNameAsync(request.Username, cancellationToken);

        string userRoleName = await _authorizationService.GetUserRoleAsync(user.Id, cancellationToken);

        if (_currentUser.CanChangeUserRole(userRoleName, request.UserRole) is false)
            throw new AccessDeniedException($"Unable to change role of {user.Username}");

        await _authorizationService.UpdateUserRoleAsync(user.Id, request.UserRole, cancellationToken);
    }
}