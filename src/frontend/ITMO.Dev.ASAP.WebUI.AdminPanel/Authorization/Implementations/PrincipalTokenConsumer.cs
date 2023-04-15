using ITMO.Dev.ASAP.WebUI.AdminPanel.Navigation;
using Microsoft.AspNetCore.Components.Routing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Authorization.Implementations;

public class PrincipalTokenConsumer : ITokenConsumer, INavigationConsumer
{
    private readonly IEnumerable<IPrincipalConsumer> _principalConsumers;
    private readonly Storage _principalStorage;

    public PrincipalTokenConsumer(IEnumerable<IPrincipalConsumer> principalConsumers, Storage principalStorage)
    {
        _principalConsumers = principalConsumers;
        _principalStorage = principalStorage;
    }

    public async ValueTask OnNextAsync(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);

        _principalStorage.Principal = new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims, "jwt"));

        foreach (IPrincipalConsumer consumer in _principalConsumers)
        {
            await consumer.OnNextAsync(_principalStorage.Principal);
        }
    }

    public async ValueTask OnExpiredAsync()
    {
        foreach (IPrincipalConsumer consumer in _principalConsumers)
        {
            await consumer.OnExpiredAsync();
        }
    }

    public async ValueTask OnNavigateAsync(NavigationContext context)
    {
        Claim? expirationClaim = _principalStorage.Principal?.Claims
            .SingleOrDefault(x => x.Type.Equals(JwtRegisteredClaimNames.Exp, StringComparison.OrdinalIgnoreCase));

        if (expirationClaim is null)
            return;

        if (int.TryParse(expirationClaim.Value, out int expiration) is false)
            return;

        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        if (now < expiration)
            return;

        foreach (IPrincipalConsumer consumer in _principalConsumers)
        {
            await consumer.OnExpiredAsync();
        }
    }

    public class Storage
    {
        public ClaimsPrincipal? Principal { get; set; }
    }
}