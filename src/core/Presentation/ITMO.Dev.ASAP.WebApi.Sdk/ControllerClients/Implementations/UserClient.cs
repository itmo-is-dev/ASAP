using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Users;
using ITMO.Dev.ASAP.WebApi.Sdk.Extensions;
using ITMO.Dev.ASAP.WebApi.Sdk.Tools;
using Newtonsoft.Json;

namespace ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients.Implementations;

internal class UserClient : IUserClient
{
    private readonly ClientRequestHandler _handler;
    private readonly JsonSerializerSettings _serializerSettings;

    public UserClient(HttpClient client, JsonSerializerSettings serializerSettings)
    {
        _serializerSettings = serializerSettings;
        _handler = new ClientRequestHandler(client, serializerSettings);
    }

    public async Task<UserDto?> FindCurrentUserAsync(CancellationToken cancellationToken)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, "api/User");
        return await _handler.SendAsync<UserDto?>(message, cancellationToken);
    }

    public async Task UpdateUniversityIdAsync(
        Guid userId,
        int universityId,
        CancellationToken cancellationToken = default)
    {
        string uri = $"api/User/{userId}/universityId/update?universityId={universityId}";
        using var message = new HttpRequestMessage(HttpMethod.Post, uri);

        await _handler.SendAsync(message, cancellationToken);
    }

    public async Task<UserDto?> FindUserByUniversityIdAsync(
        int universityId,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, $"api/User?universityId={universityId}");
        return await _handler.SendAsync<UserDto?>(message, cancellationToken);
    }

    public async Task UpdateNameAsync(
        Guid userId,
        string firstName,
        string middleName,
        string lastName,
        CancellationToken cancellationToken = default)
    {
        string uri = $"api/User/{userId}/change-name?firstName={firstName}&middleName={middleName}&lastName={lastName}";
        using var message = new HttpRequestMessage(HttpMethod.Post, uri);

        await _handler.SendAsync(message, cancellationToken);
    }

    public async Task<GetUserIdentityInfosResponse> GetUserIdentityInfosDto(
        QueryConfiguration<UserQueryParameter> queryConfiguration,
        int? page,
        CancellationToken cancellationToken = default)
    {
        string uri = page is null
            ? "api/User/identity-info"
            : $"api/User/identity-info?page={page}";

        using var message = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = queryConfiguration.ToContent(_serializerSettings),
        };

        return await _handler.SendAsync<GetUserIdentityInfosResponse>(message, cancellationToken);
    }
}