using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Identity.Abstractions.Entities;
using ITMO.Dev.ASAP.Identity.Abstractions.Services;
using ITMO.Dev.ASAP.Identity.Exceptions;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Commands.UpdatePassword;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;

internal class UpdatePasswordHandler
{
    private readonly ICurrentUser _currentUser;
    private readonly IIdentitySetvice _identitySetvice;

    public UpdatePasswordHandler(ICurrentUser currentUser, IIdentitySetvice identitySetvice)
    {
        _currentUser = currentUser;
        _identitySetvice = identitySetvice;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        AsapIdentityUser user = await _identitySetvice.GetUserByIdAsync(_currentUser.Id, cancellationToken);

        bool passwordCorrect = await _identitySetvice.CheckUserPasswordAsync(user, request.CurrentPassword, cancellationToken);

        if (passwordCorrect is false)
            throw new UpdatePasswordFailedException("Invalid password");

        if (request.NewPassword.Equals(request.CurrentPassword, StringComparison.Ordinal))
            throw new UpdatePasswordFailedException("The old password is the same as the new one");

        await _identitySetvice.UpdateUserPasswordAsync(user, request.NewPassword, cancellationToken);

        return Unit.Value;
    }
}