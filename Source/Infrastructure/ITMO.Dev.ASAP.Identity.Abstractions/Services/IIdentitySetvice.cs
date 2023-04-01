using ITMO.Dev.ASAP.Identity.Abstractions.Entities;

namespace ITMO.Dev.ASAP.Identity.Abstractions.Services;

public interface IIdentitySetvice
{
    Task AuthorizeAdminAsync(string username, CancellationToken cancellationToken = default);

    Task CreateUserAsync(Guid userId, string username, string password, string roleName, CancellationToken cancellationToken = default);

    Task<AsapIdentityUser> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<AsapIdentityUser> GetUserByNameAsync(string username, CancellationToken cancellationToken = default);

    Task UpdateUserNameAsync(AsapIdentityUser user, string username, CancellationToken cancellationToken = default);

    Task UpdateUserPasswordAsync(AsapIdentityUser user, string password, CancellationToken cancellationToken = default);

    Task UpdateUserRoleAsync(AsapIdentityUser user, string roleName, CancellationToken cancellationToken = default);

    Task<string> GetUserRoleAsync(AsapIdentityUser user, CancellationToken cancellationToken = default);

    Task<bool> CheckUserPasswordAsync(AsapIdentityUser user, string password, CancellationToken cancellationToken = default);
}