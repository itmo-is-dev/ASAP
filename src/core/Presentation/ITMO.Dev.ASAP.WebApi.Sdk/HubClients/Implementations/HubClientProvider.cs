using ITMO.Dev.ASAP.WebApi.Sdk.Identity;
using Microsoft.AspNetCore.SignalR.Client;

namespace ITMO.Dev.ASAP.WebApi.Sdk.HubClients.Implementations;

public class HubClientProvider<TClient> : IHubClientProvider<TClient> where TClient : IHubClient
{
    private readonly Func<HubConnection, TClient> _factory;

    private readonly Lazy<Task<HubConnection>> _connection;

    public HubClientProvider(Uri uri, Func<HubConnection, TClient> factory, ITokenProvider tokenProvider)
    {
        _factory = factory;
        _connection = new Lazy<Task<HubConnection>>(() => CreateClientAsync(uri, tokenProvider));
    }

    public async ValueTask<TClient> GetClientAsync(CancellationToken cancellationToken)
    {
        HubConnection connection = await _connection.Value;
        return _factory.Invoke(connection);
    }

    private static async Task<HubConnection> CreateClientAsync(Uri baseAddress, ITokenProvider identityProvider)
    {
        HubConnection connection = new HubConnectionBuilder()
            .WithUrl(baseAddress, options => options.AccessTokenProvider = () => ProvideTokenAsync(identityProvider))
            .WithAutomaticReconnect()
            .Build();

        await connection.StartAsync();

        return connection;
    }

    private static async Task<string?> ProvideTokenAsync(ITokenProvider tokenProvider)
        => await tokenProvider.FindIdentityAsync();
}