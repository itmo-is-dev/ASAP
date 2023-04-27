using ITMO.Dev.ASAP.WebUI.Abstractions.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace ITMO.Dev.ASAP.WebUI.Application.Authorization;

internal class StateProviderPrincipalConsumer : AuthenticationStateProvider, IPrincipalConsumer
{
    private ClaimsPrincipal? _principal;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var state = new AuthenticationState(_principal ?? new ClaimsPrincipal());
        return Task.FromResult(state);
    }

    public ValueTask OnNextAsync(ClaimsPrincipal principal)
    {
        _principal = principal;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        return ValueTask.CompletedTask;
    }

    public ValueTask OnExpiredAsync()
    {
        _principal = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        return ValueTask.CompletedTask;
    }
}