using Blazored.LocalStorage;
using ITMO.Dev.ASAP.WebUI.Abstractions;
using ITMO.Dev.ASAP.WebUI.Abstractions.Authorization;

namespace ITMO.Dev.ASAP.WebUI.Application.Authorization;

internal class LocalStorageTokenConsumer : ITokenConsumer
{
    private readonly ILocalStorageService _storage;

    public LocalStorageTokenConsumer(ILocalStorageService storage)
    {
        _storage = storage;
    }

    public ValueTask OnNextAsync(string token)
        => _storage.SetItemAsStringAsync(Constants.IdentityKey, token);

    public ValueTask OnExpiredAsync()
        => _storage.RemoveItemAsync(Constants.IdentityKey);
}