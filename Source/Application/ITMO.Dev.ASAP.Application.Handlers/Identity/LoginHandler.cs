using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Identity.Abstractions.Models;
using ITMO.Dev.ASAP.Identity.Abstractions.Services;
using ITMO.Dev.ASAP.Identity.Tools;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Queries.Login;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;

internal class LoginHandler : IRequestHandler<Query, Response>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IdentityConfiguration _configuration;

    public LoginHandler(IAuthorizationService authorizationService, IdentityConfiguration configuration)
    {
        _authorizationService = authorizationService;
        _configuration = configuration;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        AsapIdentityUserDto user = await _authorizationService.GetUserByNameAsync(request.Username, cancellationToken);

        bool passwordCorrect = await _authorizationService.CheckUserPasswordAsync(user.Id, request.Password, cancellationToken);

        if (passwordCorrect is false)
            throw new UnauthorizedException("You are not authorized");

        string userRole = await _authorizationService.GetUserRoleAsync(user.Id, cancellationToken);

        IEnumerable<Claim> claims = new List<Claim>()
            .Append(new Claim(ClaimTypes.Role, userRole))
            .Append(new Claim(ClaimTypes.Name, user.Username))
            .Append(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()))
            .Append(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

        JwtSecurityToken token = GetToken(claims);
        string? tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new Response(
            tokenString,
            token.ValidTo,
            new List<string> { userRole });
    }

    private JwtSecurityToken GetToken(IEnumerable<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Secret));

        var token = new JwtSecurityToken(
            _configuration.Issuer,
            _configuration.Audience,
            expires: DateTime.UtcNow.AddHours(_configuration.ExpiresHours),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

        return token;
    }
}