using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace ITMO.Dev.ASAP.Identity.Extensions;

internal static class UserManagerExtensions
{
    public static async Task<AsapIdentityUser> GetByIdAsync(
        this UserManager<AsapIdentityUser> userManager,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        AsapIdentityUser? user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            throw new EntityNotFoundException($"User with id {userId} was not found");

        return user;
    }

    public static async Task<AsapIdentityUser> GetByNameAsync(
        this UserManager<AsapIdentityUser> userManager,
        string username,
        CancellationToken cancellationToken = default)
    {
        AsapIdentityUser? user = await userManager.FindByNameAsync(username);

        if (user is null)
            throw new EntityNotFoundException($"User with username {username} was not found");

        return user;
    }
}
