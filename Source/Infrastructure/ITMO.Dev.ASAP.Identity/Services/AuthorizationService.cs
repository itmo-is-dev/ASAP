using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Common.Exceptions;
using ITMO.Dev.ASAP.Application.Dto.Identity;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Identity.Entities;
using ITMO.Dev.ASAP.Identity.Extensions;
using ITMO.Dev.ASAP.Identity.Tools;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ITMO.Dev.ASAP.Identity.Services;

internal class AuthorizationService : IAuthorizationService
{
    private readonly UserManager<AsapIdentityUser> _userManager;
    private readonly RoleManager<AsapIdentityRole> _roleManager;
    private readonly IdentityConfiguration _configuration;

    public AuthorizationService(
        UserManager<AsapIdentityUser> userManager,
        RoleManager<AsapIdentityRole> roleManager,
        IdentityConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
        _roleManager = roleManager;
    }

    public async Task AuthorizeAdminAsync(string username, CancellationToken cancellationToken = default)
    {
        AsapIdentityUser? user = await _userManager.FindByNameAsync(username);

        if (user is not null && await _userManager.IsInRoleAsync(user, AsapIdentityRoleNames.AdminRoleName))
            return;

        throw new UnauthorizedException("User is not admin");
    }

    public async Task CreateRoleIfNotExistsAsync(string roleName, CancellationToken cancellationToken = default)
    {
        await _roleManager.CreateIfNotExistsAsync(roleName, cancellationToken);
    }

    public async Task<IdentityUserDto> CreateUserAsync(
        Guid userId,
        string username,
        string password,
        string roleName,
        CancellationToken cancellationToken = default)
    {
        var user = new AsapIdentityUser
        {
            Id = userId,
            UserName = username,
            SecurityStamp = Guid.NewGuid().ToString(),
        };

        IdentityResult result = await _userManager.CreateAsync(user, password);

        result.EnsureSucceeded();

        await _userManager.AddToRoleAsync(user, roleName);

        return user.ToDto();
    }

    public async Task<IdentityUserDto> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        AsapIdentityUser user = await _userManager.GetByIdAsync(userId, cancellationToken);

        return user.ToDto();
    }

    public async Task<IReadOnlyCollection<IdentityUserDto>> GetUsersByIdsAsync(
        IEnumerable<Guid> userIds,
        CancellationToken cancellationToken = default)
    {
        List<AsapIdentityUser> users = await _userManager.Users
            .Where(x => userIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        IReadOnlyCollection<IdentityUserDto> userDtos = users
            .Select(x => x.ToDto())
            .ToList();

        return userDtos;
    }

    public async Task<IdentityUserDto> GetUserByNameAsync(string username, CancellationToken cancellationToken = default)
    {
        AsapIdentityUser user = await _userManager.GetByNameAsync(username, cancellationToken);

        return user.ToDto();
    }

    public async Task UpdateUserNameAsync(Guid userId, string newUsername, CancellationToken cancellationToken = default)
    {
        AsapIdentityUser user = await _userManager.GetByIdAsync(userId, cancellationToken);

        user.UserName = newUsername;

        IdentityResult? result = await _userManager.UpdateAsync(user);

        result.EnsureSucceeded();
    }

    public async Task<IdentityUserDto> UpdateUserPasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        if (currentPassword.Equals(newPassword, StringComparison.Ordinal))
            throw new IdentityOperationNotSucceededException("New password cannot be equal to old password");

        AsapIdentityUser user = await _userManager.GetByIdAsync(userId, cancellationToken);

        IdentityResult result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        result.EnsureSucceeded();

        return user.ToDto();
    }

    public async Task UpdateUserRoleAsync(Guid userId, string newRoleName, CancellationToken cancellationToken = default)
    {
        AsapIdentityUser user = await _userManager.GetByIdAsync(userId, cancellationToken);
        IList<string> roles = await _userManager.GetRolesAsync(user);

        await _userManager.RemoveFromRolesAsync(user, roles);
        await _userManager.AddToRoleAsync(user, newRoleName);
    }

    public async Task<string> GetUserRoleAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        AsapIdentityUser user = await _userManager.GetByIdAsync(userId, cancellationToken);
        IList<string> roles = await _userManager.GetRolesAsync(user);

        return roles.Single();
    }

    public async Task<bool> CheckUserPasswordAsync(
        Guid userId,
        string password,
        CancellationToken cancellationToken = default)
    {
        AsapIdentityUser user = await _userManager.GetByIdAsync(userId, cancellationToken);

        return await _userManager.CheckPasswordAsync(user, password);
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