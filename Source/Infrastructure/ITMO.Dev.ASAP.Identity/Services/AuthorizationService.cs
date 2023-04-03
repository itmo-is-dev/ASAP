using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Dto.Identity;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Identity.Entities;
using ITMO.Dev.ASAP.Identity.Exceptions;
using ITMO.Dev.ASAP.Identity.Extensions;
using Microsoft.AspNetCore.Identity;

namespace ITMO.Dev.ASAP.Identity.Services;

internal class AuthorizationService : IAuthorizationService
{
    private readonly UserManager<AsapIdentityUser> _userManager;
    private readonly RoleManager<AsapIdentityRole> _roleManager;

    public AuthorizationService(
        UserManager<AsapIdentityUser> userManager,
        RoleManager<AsapIdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public IReadOnlyCollection<IdentityUserDto> Users => _userManager.Users
        .Select(x => x.ToDto())
        .ToList();

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

    public async Task<IdentityUserDto> CreateUserAsync(Guid userId, string username, string password, string roleName, CancellationToken cancellationToken = default)
    {
        var user = new AsapIdentityUser
        {
            Id = userId,
            UserName = username,
            SecurityStamp = Guid.NewGuid().ToString(),
        };

        IdentityResult result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded is false)
            throw new RegistrationFailedException(string.Join(' ', result.Errors.Select(x => x.Description)));

        await _userManager.AddToRoleAsync(user, roleName);

        return user.ToDto();
    }

    public async Task<IdentityUserDto> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        AsapIdentityUser user = await _userManager.GetByIdAsync(userId, cancellationToken);

        return user.ToDto();
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

        if (result.Succeeded is false)
            throw new UpdateUsernameFailedException(string.Join(' ', result.Errors.Select(r => r.Description)));
    }

    public async Task UpdateUserPasswordAsync(Guid userId, string newPassword, CancellationToken cancellationToken = default)
    {
        AsapIdentityUser user = await _userManager.GetByIdAsync(userId, cancellationToken);

        await _userManager.AddPasswordAsync(user, newPassword);

        IdentityResult result = await _userManager.UpdateAsync(user);

        if (result.Succeeded is false)
            throw new UpdatePasswordFailedException(string.Join(' ', result.Errors.Select(r => r.Description)));
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

    public async Task<bool> CheckUserPasswordAsync(Guid userId, string password, CancellationToken cancellationToken = default)
    {
        AsapIdentityUser user = await _userManager.GetByIdAsync(userId, cancellationToken);

        bool passwordCorrect = await _userManager.CheckPasswordAsync(user, password);

        return passwordCorrect;
    }
}