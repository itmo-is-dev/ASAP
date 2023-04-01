using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Identity.Abstractions.Entities;
using ITMO.Dev.ASAP.Identity.Abstractions.Services;
using ITMO.Dev.ASAP.Identity.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace ITMO.Dev.ASAP.Identity.Services;

public class IdentitySetvice : IIdentitySetvice
{
    private readonly UserManager<AsapIdentityUser> _userManager;

    public IdentitySetvice(UserManager<AsapIdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task AuthorizeAdminAsync(string username, CancellationToken cancellationToken = default)
    {
        AsapIdentityUser? user = await _userManager.FindByNameAsync(username);

        if (user is not null && await _userManager.IsInRoleAsync(user, AsapIdentityRole.AdminRoleName))
            return;

        throw new UnauthorizedException("User is not admin");
    }

    public async Task CreateUserAsync(Guid userId, string username, string password, string roleName, CancellationToken cancellationToken = default)
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
    }

    public async Task<AsapIdentityUser> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        AsapIdentityUser? user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            throw new EntityNotFoundException($"User with id {userId} was not found");

        return user;
    }

    public async Task<AsapIdentityUser> GetUserByNameAsync(string username, CancellationToken cancellationToken = default)
    {
        AsapIdentityUser? user = await _userManager.FindByNameAsync(username);

        if (user is null)
            throw new EntityNotFoundException($"User with username {username} was not found");

        return user;
    }

    public async Task UpdateUserNameAsync(AsapIdentityUser user, string username, CancellationToken cancellationToken = default)
    {
        user.UserName = username;

        IdentityResult? result = await _userManager.UpdateAsync(user);

        if (result.Succeeded is false)
            throw new UpdateUsernameFailedException(string.Join(' ', result.Errors.Select(r => r.Description)));
    }

    public async Task UpdateUserPasswordAsync(AsapIdentityUser user, string password, CancellationToken cancellationToken = default)
    {
        await _userManager.AddPasswordAsync(user, password);

        IdentityResult result = await _userManager.UpdateAsync(user);

        if (result.Succeeded is false)
            throw new UpdatePasswordFailedException(string.Join(' ', result.Errors.Select(r => r.Description)));
    }

    public async Task UpdateUserRoleAsync(AsapIdentityUser user, string roleName, CancellationToken cancellationToken = default)
    {
        IList<string> roles = await _userManager.GetRolesAsync(user);

        await _userManager.RemoveFromRolesAsync(user, roles);
        await _userManager.AddToRoleAsync(user, roleName);
    }

    public async Task<string> GetUserRoleAsync(AsapIdentityUser user, CancellationToken cancellationToken = default)
    {
        IList<string> roles = await _userManager.GetRolesAsync(user);

        return roles.Single();
    }

    public async Task<bool> CheckUserPasswordAsync(AsapIdentityUser user, string password, CancellationToken cancellationToken = default)
    {
        bool passwordCorrect = await _userManager.CheckPasswordAsync(user, password);

        return passwordCorrect;
    }
}