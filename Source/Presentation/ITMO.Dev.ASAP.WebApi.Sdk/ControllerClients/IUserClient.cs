using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Users;

namespace ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;

public interface IUserClient
{
    Task<UserDto?> FindCurrentUserAsync(CancellationToken cancellationToken);

    Task UpdateUniversityIdAsync(Guid userId, int universityId, CancellationToken cancellationToken = default);

    Task<UserDto?> FindUserByUniversityIdAsync(int universityId, CancellationToken cancellationToken = default);

    Task UpdateNameAsync(
        Guid userId,
        string firstName,
        string middleName,
        string lastName,
        CancellationToken cancellationToken = default);

    Task<GetUserIdentityInfosResponse> GetUserIdentityInfosDto(
        QueryConfiguration<UserQueryParameter> queryConfiguration,
        int? page = null,
        CancellationToken cancellationToken = default);
}