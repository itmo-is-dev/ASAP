namespace ITMO.Dev.ASAP.WebApi.Sdk.HubClients;

public interface IHubClientProvider<TClient> where TClient : IHubClient
{
    ValueTask<TClient> GetClientAsync(CancellationToken cancellationToken = default);
}