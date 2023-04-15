using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Dto.Identity;
using ITMO.Dev.ASAP.Common.Exceptions;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Queries.Login;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;

internal class LoginHandler : IRequestHandler<Query, Response>
{
    private readonly IAuthorizationService _authorizationService;

    public LoginHandler(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        IdentityUserDto user = await _authorizationService.GetUserByNameAsync(request.Username, cancellationToken);

        bool passwordCorrect = await _authorizationService.CheckUserPasswordAsync(
            user.Id,
            request.Password,
            cancellationToken);

        if (passwordCorrect is false)
            throw new UnauthorizedException("You are not authorized");

        string token = await _authorizationService.GetUserTokenAsync(request.Username, cancellationToken);

        return new Response(token);
    }
}