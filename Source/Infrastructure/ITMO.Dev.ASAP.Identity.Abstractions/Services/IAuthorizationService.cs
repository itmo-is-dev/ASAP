using ITMO.Dev.ASAP.Identity.Abstractions.Models;

namespace ITMO.Dev.ASAP.Identity.Abstractions.Services;

public interface IAuthorizationService
{
    Task AuthorizeAdminAsync(string username, CancellationToken cancellationToken = default);

    Task CreateRoleIfNotExistsAsync(string roleName, CancellationToken cancellationToken = default);

    Task<AsapIdentityUserDto> CreateUserAsync(Guid userId, string username, string password, string roleName, CancellationToken cancellationToken = default);

    Task<AsapIdentityUserDto> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<AsapIdentityUserDto> GetUserByNameAsync(string username, CancellationToken cancellationToken = default);

    Task UpdateUserNameAsync(Guid userId, string newUsername, CancellationToken cancellationToken = default);

    Task UpdateUserPasswordAsync(Guid userId, string newPassword, CancellationToken cancellationToken = default);

    Task UpdateUserRoleAsync(Guid userId, string newRoleName, CancellationToken cancellationToken = default);

    Task<string> GetUserRoleAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<bool> CheckUserPasswordAsync(Guid userId, string password, CancellationToken cancellationToken = default);
}