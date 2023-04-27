using ITMO.Dev.ASAP.WebApi.Sdk.Identity;
using ITMO.Dev.ASAP.WebUI.Abstractions.Authorization;

namespace ITMO.Dev.ASAP.WebUI.Application.Authorization;

internal class TokenProviderConsumer : ITokenConsumer, ITokenProvider
{
    private readonly Storage _tokenStorage;

    public TokenProviderConsumer(Storage tokenStorage)
    {
        _tokenStorage = tokenStorage;
    }

    public ValueTask OnNextAsync(string token)
    {
        _tokenStorage.Token = token;
        return ValueTask.CompletedTask;
    }

    public ValueTask OnExpiredAsync()
    {
        _tokenStorage.Token = null;
        return ValueTask.CompletedTask;
    }

    public ValueTask<string?> FindIdentityAsync(CancellationToken cancellationToken = default)
        => ValueTask.FromResult(_tokenStorage.Token);

    public class Storage
    {
        public string? Token { get; set; }
    }
}