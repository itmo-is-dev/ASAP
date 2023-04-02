using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Identity.Entities;
using ITMO.Dev.ASAP.Identity.Tools;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ITMO.Dev.ASAP.Identity.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly UserManager<AsapIdentityUser> _userManager;
    private readonly IdentityConfiguration _configuration;

    public AuthorizationService(UserManager<AsapIdentityUser> userManager, IdentityConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task AuthorizeAdminAsync(string username, CancellationToken cancellationToken)
    {
        AsapIdentityUser? user = await _userManager.FindByNameAsync(username);

        if (user is not null && await _userManager.IsInRoleAsync(user, AsapIdentityRole.AdminRoleName))
            return;

        throw new UnauthorizedException("User is not admin");
    }

    public async Task<string> GetUserTokenAsync(string username, CancellationToken cancellationToken)
    {
        AsapIdentityUser? user = await _userManager.FindByNameAsync(username);
        IList<string> roles = await _userManager.GetRolesAsync(user);

        IEnumerable<Claim> claims = roles
            .Select(userRole => new Claim(ClaimTypes.Role, userRole))
            .Append(new Claim(ClaimTypes.Name, user.UserName))
            .Append(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()))
            .Append(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Secret));

        var token = new JwtSecurityToken(
            _configuration.Issuer,
            _configuration.Audience,
            expires: DateTime.UtcNow.AddHours(_configuration.ExpiresHours),
            claims: claims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}