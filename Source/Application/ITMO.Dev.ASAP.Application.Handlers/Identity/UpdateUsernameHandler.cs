using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Dto.Identity;
using ITMO.Dev.ASAP.Common.Exceptions;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Commands.UpdateUsername;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;

internal class UpdateUsernameHandler : IRequestHandler<Command, Response>
{
    private readonly ICurrentUser _currentUser;
    private readonly IAuthorizationService _authorizationService;

    public UpdateUsernameHandler(ICurrentUser currentUser, IAuthorizationService authorizationService)
    {
        _currentUser = currentUser;
        _authorizationService = authorizationService;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        IdentityUserDto user = await _authorizationService.GetUserByIdAsync(_currentUser.Id, cancellationToken);

        if (user.Username.Equals(request.Username, StringComparison.Ordinal))
            throw new UpdateUsernameFailedException("the old username is the same as the new one");

        await _authorizationService.UpdateUserNameAsync(user.Id, request.Username, cancellationToken);

        string token = await _authorizationService.GetUserTokenAsync(request.Username, cancellationToken);

        return new Response(token);
    }
}