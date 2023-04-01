using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Common.Exceptions;
using ITMO.Dev.ASAP.Identity.Abstractions.Entities;
using ITMO.Dev.ASAP.Identity.Abstractions.Services;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Commands.ChangeUserRole;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;

internal class ChangeUserRoleHandler : IRequestHandler<Command>
{
    private readonly ICurrentUser _currentUser;
    private readonly IIdentitySetvice _identitySetvice;

    public ChangeUserRoleHandler(ICurrentUser currentUser, IIdentitySetvice identitySetvice)
    {
        _currentUser = currentUser;
        _identitySetvice = identitySetvice;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        AsapIdentityUser user = await _identitySetvice.GetUserByNameAsync(request.Username, cancellationToken);
        string userRoleName = await _identitySetvice.GetUserRoleAsync(user, cancellationToken);

        if (_currentUser.CanChangeUserRole(userRoleName, request.UserRole) is false)
            throw new AccessDeniedException($"Unable to change role of {user.UserName}");

        await _identitySetvice.UpdateUserRoleAsync(user, request.UserRole, cancellationToken);

        return Unit.Value;
    }
}