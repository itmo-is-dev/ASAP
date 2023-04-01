using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Identity.Abstractions.Entities;
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
    private readonly IIdentitySetvice _identitySetvice;
    private readonly IdentityConfiguration _configuration;

    public LoginHandler(IIdentitySetvice identitySetvice, IdentityConfiguration configuration)
    {
        _identitySetvice = identitySetvice;
        _configuration = configuration;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        AsapIdentityUser user = await _identitySetvice.GetUserByNameAsync(request.Username, cancellationToken);

        bool passwordCorrect = await _identitySetvice.CheckUserPasswordAsync(user, request.Password, cancellationToken);

        if (passwordCorrect is false)
            throw new UnauthorizedException("You are not authorized");

        string userRole = await _identitySetvice.GetUserRoleAsync(user, cancellationToken);

        IEnumerable<Claim> claims = new List<Claim>()
            .Append(new Claim(ClaimTypes.Role, userRole))
            .Append(new Claim(ClaimTypes.Name, user.UserName ?? string.Empty))
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