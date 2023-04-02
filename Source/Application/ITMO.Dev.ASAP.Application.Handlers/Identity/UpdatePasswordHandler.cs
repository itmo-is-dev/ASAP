using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Common.Exceptions;
using ITMO.Dev.ASAP.Identity.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Commands.UpdatePassword;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;

internal class UpdatePasswordHandler : IRequestHandler<Command, Response>
{
    private readonly UserManager<AsapIdentityUser> _userManager;
    private readonly ICurrentUser _currentUser;
    private readonly IAuthorizationService _authorizationService;

    public UpdatePasswordHandler(
        UserManager<AsapIdentityUser> userManager,
        ICurrentUser currentUser,
        IAuthorizationService authorizationService)
    {
        _userManager = userManager;
        _currentUser = currentUser;
        _authorizationService = authorizationService;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        AsapIdentityUser? existingUser = await _userManager.FindByIdAsync(_currentUser.Id.ToString());

        IdentityResult? result = await _userManager
            .ChangePasswordAsync(existingUser, request.CurrentPassword, request.NewPassword);

        if (result.Succeeded is false)
            throw new UpdatePasswordFailedException(string.Join(' ', result.Errors.Select(r => r.Description)));

        string token = await _authorizationService.GetUserTokenAsync(existingUser.UserName, cancellationToken);

        return new Response(token);
    }
}