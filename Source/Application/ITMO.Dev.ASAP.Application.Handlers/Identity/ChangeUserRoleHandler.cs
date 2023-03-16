using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Identity.Entities;
using ITMO.Dev.ASAP.Identity.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Commands.ChangeUserRole;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;

internal class ChangeUserRoleHandler : IRequestHandler<Command>
{
    private readonly ICurrentUser _currentUser;
    private readonly UserManager<AsapIdentityUser> _userManager;

    public ChangeUserRoleHandler(
        ICurrentUser currentUser,
        UserManager<AsapIdentityUser> userManager)
    {
        _currentUser = currentUser;
        _userManager = userManager;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        AsapIdentityUser user = await _userManager.GetByNameAsync(request.Username);
        IList<string> userRoles = await _userManager.GetRolesAsync(user);
        string userRoleName = userRoles.Single();

        if (_currentUser.CanChangeUserRole(userRoleName, request.UserRole) is false)
            throw new AsapIdentityException($"Unable to change role of {user.UserName}");

        await _userManager.RemoveFromRolesAsync(user, new[] { userRoleName });
        await _userManager.AddToRoleAsync(user, request.UserRole);

        return Unit.Value;
    }
}