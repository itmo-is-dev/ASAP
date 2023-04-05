using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Dto.Identity;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Commands.UpdatePassword;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;

internal class UpdatePasswordHandler : IRequestHandler<Command, Response>
{
    private readonly ICurrentUser _currentUser;
    private readonly IAuthorizationService _authorizationService;

    public UpdatePasswordHandler(ICurrentUser currentUser, IAuthorizationService authorizationService)
    {
        _currentUser = currentUser;
        _authorizationService = authorizationService;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        IdentityUserDto existingUser = await _authorizationService.UpdateUserPasswordAsync(
            _currentUser.Id,
            request.CurrentPassword,
            request.NewPassword,
            cancellationToken);

        string token = await _authorizationService.GetUserTokenAsync(existingUser.Username, cancellationToken);

        return new Response(token);
    }
}