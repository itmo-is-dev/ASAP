using Blazored.LocalStorage;
using ITMO.Dev.ASAP.WebUI.Abstractions;
using ITMO.Dev.ASAP.WebUI.Abstractions.Authorization;

namespace ITMO.Dev.ASAP.WebUI.Application.Authorization;

internal class LocalStorageAuthorizationLoader : IAuthorizationLoader
{
    private readonly IEnumerable<ITokenConsumer> _tokenConsumers;
    private readonly ILocalStorageService _localStorageService;

    public LocalStorageAuthorizationLoader(
        IEnumerable<ITokenConsumer> tokenConsumers,
        ILocalStorageService localStorageService)
    {
        _tokenConsumers = tokenConsumers;
        _localStorageService = localStorageService;
    }

    public async ValueTask LoadAsync()
    {
        string? token = await _localStorageService.GetItemAsStringAsync(Constants.IdentityKey);

        if (token is null)
            return;

        foreach (ITokenConsumer consumer in _tokenConsumers)
        {
            await consumer.OnNextAsync(token);
        }
    }
}