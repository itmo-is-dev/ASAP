using ITMO.Dev.ASAP.Application.Dto.Identity;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Identity;

namespace ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;

public interface IIdentityClient
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task ChangeUserRoleAsync(string username, string roleName, CancellationToken cancellationToken = default);

    Task CreateUserAccountAsync(
        Guid id,
        CreateUserAccountRequest request,
        CancellationToken cancellationToken = default);

    Task<UpdatePasswordResponse> UpdatePasswordAsync(
        UpdatePasswordRequest request,
        CancellationToken cancellationToken = default);

    Task<UpdateUsernameResponse> UpdateUsernameAsync(
        UpdateUsernameRequest request,
        CancellationToken cancellationToken = default);

    Task<PasswordOptionsDto> GetPasswordOptionsAsync(CancellationToken cancellationToken = default);
}