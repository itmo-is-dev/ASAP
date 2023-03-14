using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Identity;

namespace ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;

public interface IIdentityClient
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task ChangeUserRoleAsync(string username, ChangeUserRoleRequest request, CancellationToken cancellationToken = default);

    Task<LoginResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);

    Task CreateUserAccountAsync(Guid id, CreateUserAccountRequest request, CancellationToken cancellationToken = default);
}