using Microsoft.AspNetCore.SignalR.Client;

namespace ITMO.Dev.ASAP.WebApi.Sdk.HubClients;

public interface IHubConnectionCreatable<T>
{
    static abstract T Create(IServiceProvider provider, HubConnection connection);
}