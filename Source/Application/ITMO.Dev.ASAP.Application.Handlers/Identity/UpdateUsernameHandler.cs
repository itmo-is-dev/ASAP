using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Identity.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Commands.UpdateUsername;

namespace ITMO.Dev.ASAP.Application.Handlers.Users;

internal class UpdateUsernameHandler : IRequestHandler<Command>
{
    private readonly UserManager<AsapIdentityUser> _userManager;
    private readonly ICurrentUser _currentUser;

    public UpdateUsernameHandler(UserManager<AsapIdentityUser> userManager, ICurrentUser currentUser)
    {
        _userManager = userManager;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        AsapIdentityUser? existingUser = await _userManager.FindByIdAsync(_currentUser.Id.ToString());

        if (existingUser.UserName.Equals(request.Username, StringComparison.Ordinal))
            throw new UpdateUsernameFailedException("the old username is the same as the new one");

        existingUser.UserName = request.Username;

        IdentityResult? result = await _userManager.UpdateAsync(existingUser);

        if (result.Succeeded is false)
            throw new UpdateUsernameFailedException(string.Join(' ', result.Errors.Select(r => r.Description)));

        return Unit.Value;
    }
}