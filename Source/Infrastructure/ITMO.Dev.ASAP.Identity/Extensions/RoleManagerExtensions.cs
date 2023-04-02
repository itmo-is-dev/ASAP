using ITMO.Dev.ASAP.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace ITMO.Dev.ASAP.Identity.Extensions;

internal static class RoleManagerExtensions
{
    public static async Task CreateIfNotExistsAsync(
        this RoleManager<AsapIdentityRole> roleManager,
        string roleName,
        CancellationToken cancellationToken = default)
    {
        bool roleExists = await roleManager.RoleExistsAsync(roleName);

        if (roleExists is false)
            await roleManager.CreateAsync(new AsapIdentityRole(roleName));
    }
}