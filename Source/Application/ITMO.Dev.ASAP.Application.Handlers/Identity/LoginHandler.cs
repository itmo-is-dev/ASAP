using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Identity.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Queries.Login;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;

internal class LoginHandler : IRequestHandler<Query, Response>
{
    private readonly UserManager<AsapIdentityUser> _userManager;
    private readonly IAuthorizationService _authorizationService;

    public LoginHandler(
        UserManager<AsapIdentityUser> userManager,
        IAuthorizationService authorizationService)
    {
        _userManager = userManager;
        _authorizationService = authorizationService;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        AsapIdentityUser? user = await _userManager.FindByNameAsync(request.Username);

        if (user is null)
            throw new UnauthorizedException("You are not authorized");

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedException("You are not authorized");

        string token = await _authorizationService.GetUserTokenAsync(request.Username, cancellationToken);

        return new Response(token);
    }
}