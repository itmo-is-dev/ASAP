using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Identity;

namespace ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;

public interface IIdentityClient
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task PromoteAsync(string username, CancellationToken cancellationToken = default);

    Task<LoginResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);
    Task UpdatePasswordAsync(UpdatePasswordRequest request, CancellationToken cancellationToken = default);
    Task UpdateUsernameAsync(UpdateUsernameRequest request, CancellationToken cancellationToken = default);
}