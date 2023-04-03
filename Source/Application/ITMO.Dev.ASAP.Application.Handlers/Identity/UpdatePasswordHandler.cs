using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Dto.Identity;
using ITMO.Dev.ASAP.Identity.Exceptions;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Commands.UpdatePassword;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;

internal class UpdatePasswordHandler
{
    private readonly ICurrentUser _currentUser;
    private readonly IAuthorizationService _authorizationService;

    public UpdatePasswordHandler(ICurrentUser currentUser, IAuthorizationService authorizationService)
    {
        _currentUser = currentUser;
        _authorizationService = authorizationService;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        IdentityUserDto user = await _authorizationService.GetUserByIdAsync(_currentUser.Id, cancellationToken);

        bool passwordCorrect = await _authorizationService.CheckUserPasswordAsync(user.Id, request.CurrentPassword, cancellationToken);

        if (passwordCorrect is false)
            throw new UpdatePasswordFailedException("Invalid password");

        if (request.NewPassword.Equals(request.CurrentPassword, StringComparison.Ordinal))
            throw new UpdatePasswordFailedException("The old password is the same as the new one");

        await _authorizationService.UpdateUserPasswordAsync(user.Id, request.NewPassword, cancellationToken);

        return Unit.Value;
    }
}