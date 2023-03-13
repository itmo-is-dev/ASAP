using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace ITMO.Dev.ASAP.Identity.Extensions;

public static class UserManagerExtensions
{
    public static async Task CreateOrElseThrowAsync(
        this UserManager<AsapIdentityUser> userManager,
        AsapIdentityUser user,
        string password)
    {
        IdentityResult result = await userManager.CreateAsync(user, password);

        if (result.Succeeded is false)
            throw new RegistrationFailedException(string.Join(' ', result.Errors.Select(x => x.Description)));
    }

    public static async Task<AsapIdentityUser> GetByNameAsync(this UserManager<AsapIdentityUser> userManager, string userName)
    {
        AsapIdentityUser? user = await userManager.FindByNameAsync(userName);

        if (user is null)
            throw new EntityNotFoundException($"User with username '{userName}' does not exist");

        return user;
    }
}
