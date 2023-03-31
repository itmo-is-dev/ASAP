using Blazored.LocalStorage;
using ITMO.Dev.ASAP.WebUI.AdminPanel.Tools;

namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Authorization.Implementations;

public class LocalStorageTokenConsumer : ITokenConsumer
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