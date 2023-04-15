using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Identity;
using ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;

namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Authorization.Implementations;

public class PrincipalService : IPrincipalService
{
    private readonly IEnumerable<ITokenConsumer> _consumers;
    private readonly IIdentityClient _identityClient;

    public PrincipalService(IEnumerable<ITokenConsumer> consumers, IIdentityClient identityClient)
    {
        _identityClient = identityClient;
        _consumers = consumers;
    }

    public async Task LoginAsync(string username, string password, CancellationToken cancellationToken)
    {
        var request = new LoginRequest(username, password);
        LoginResponse response = await _identityClient.LoginAsync(request, cancellationToken);

        foreach (ITokenConsumer consumer in _consumers)
        {
            await consumer.OnNextAsync(response.Token);
        }
    }

    public async Task LogoutAsync(CancellationToken cancellationToken)
    {
        foreach (ITokenConsumer consumer in _consumers)
        {
            await consumer.OnExpiredAsync();
        }
    }

    public async Task UpdateUsernameAsync(string username, CancellationToken cancellationToken)
    {
        var request = new UpdateUsernameRequest(username);
        UpdateUsernameResponse response = await _identityClient.UpdateUsernameAsync(request, cancellationToken);

        foreach (ITokenConsumer consumer in _consumers)
        {
            await consumer.OnNextAsync(response.Token);
        }
    }

    public async Task UpdatePasswordAsync(string currentPassword, string newPassword, CancellationToken cancellationToken)
    {
        var request = new UpdatePasswordRequest(currentPassword, newPassword);
        UpdatePasswordResponse response = await _identityClient.UpdatePasswordAsync(request, cancellationToken);

        foreach (ITokenConsumer consumer in _consumers)
        {
            await consumer.OnNextAsync(response.Token);
        }
    }
}