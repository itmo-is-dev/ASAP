using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Identity.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Commands.UpdatePassword;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;
internal class UpdatePasswordHandler
{
    private readonly UserManager<AsapIdentityUser> _userManager;
    private readonly ICurrentUser _currentUser;

    public UpdatePasswordHandler(UserManager<AsapIdentityUser> userManager, ICurrentUser currentUser)
    {
        _userManager = userManager;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        AsapIdentityUser? existingUser = await _userManager.FindByIdAsync(_currentUser.Id.ToString());

        if (_userManager.CheckPasswordAsync(existingUser, request.CurrentPassword) != null)
            throw new UpdatePasswordFailedException("invalid password");

        if (request.CurrentPassword == request.NewPassword)
            throw new UpdatePasswordFailedException("the old password is the same as the new one");

        await _userManager.AddPasswordAsync(existingUser, request.NewPassword);

        IdentityResult? result = await _userManager.UpdateAsync(existingUser);

        if (!result.Succeeded)
            throw new UpdatePasswordFailedException(string.Join(' ', result.Errors.Select(r => r.Description)));

        return Unit.Value;
    }
}