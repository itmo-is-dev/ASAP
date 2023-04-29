using ITMO.Dev.ASAP.WebApi.Sdk.Identity;
using Microsoft.AspNetCore.SignalR.Client;

namespace ITMO.Dev.ASAP.WebApi.Sdk.HubClients.Implementations;

public class HubClientProvider<TClient> : IHubClientProvider<TClient>, IAsyncDisposable
    where TClient : IHubClient
{
    private readonly Func<HubConnection, TClient> _factory;

    private readonly Lazy<Task<TClient>> _instance;

    public HubClientProvider(Uri uri, Func<HubConnection, TClient> factory, ITokenProvider tokenProvider)
    {
        _factory = factory;
        _instance = new Lazy<Task<TClient>>(() => CreateClientAsync(uri, tokenProvider));
    }

    public async ValueTask<TClient> GetClientAsync(CancellationToken cancellationToken)
    {
        return await _instance.Value;
    }

    public async ValueTask DisposeAsync()
    {
        if (_instance.IsValueCreated)
        {
            TClient client = await _instance.Value;
            await client.DisposeAsync();
        }
    }

    private static async Task<string?> ProvideTokenAsync(ITokenProvider tokenProvider)
        => await tokenProvider.FindIdentityAsync();

    private async Task<TClient> CreateClientAsync(Uri baseAddress, ITokenProvider identityProvider)
    {
        HubConnection connection = new HubConnectionBuilder()
            .WithUrl(baseAddress, options => options.AccessTokenProvider = () => ProvideTokenAsync(identityProvider))
            .WithAutomaticReconnect()
            .Build();

        await connection.StartAsync();

        return _factory.Invoke(connection);
    }
}