using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Dto.Identity;
using ITMO.Dev.ASAP.Common.Exceptions;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Commands.UpdateUsername;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;

internal class UpdateUsernameHandler : IRequestHandler<Command>
{
    private readonly ICurrentUser _currentUser;
    private readonly IAuthorizationService _authorizationService;

    public UpdateUsernameHandler(ICurrentUser currentUser, IAuthorizationService authorizationService)
    {
        _currentUser = currentUser;
        _authorizationService = authorizationService;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        IdentityUserDto user = await _authorizationService.GetUserByIdAsync(_currentUser.Id, cancellationToken);

        if (user.Username.Equals(request.Username, StringComparison.Ordinal))
            throw new UpdateUsernameFailedException("the old username is the same as the new one");

        await _authorizationService.UpdateUserNameAsync(user.Id, request.Username, cancellationToken);

        return Unit.Value;
    }
}