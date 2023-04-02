using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Identity.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Commands.UpdateUsername;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;

internal class UpdateUsernameHandler : IRequestHandler<Command, Response>
{
    private readonly UserManager<AsapIdentityUser> _userManager;
    private readonly ICurrentUser _currentUser;
    private readonly IAuthorizationService _authorizationService;

    public UpdateUsernameHandler(
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

        if (existingUser.UserName.Equals(request.Username, StringComparison.Ordinal))
            throw new UpdateUsernameFailedException("the old username is the same as the new one");

        existingUser.UserName = request.Username;

        IdentityResult? result = await _userManager.UpdateAsync(existingUser);

        if (result.Succeeded is false)
            throw new UpdateUsernameFailedException(string.Join(' ', result.Errors.Select(r => r.Description)));

        string token = await _authorizationService.GetUserTokenAsync(request.Username, cancellationToken);

        return new Response(token);
    }
}