using Blazored.LocalStorage;
using ITMO.Dev.ASAP.WebApi.Sdk.Models;
using ITMO.Dev.ASAP.WebUI.AdminPanel.Tools;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Identity;

public class LocalStorageIdentityManager : IIdentityManager
{
    private const string IdentityKey = "AsapIdentity";

    private static readonly IEqualityComparer<string> OrdinalIgnoreCaseComparer = EqualityComparerFactory
        .Create<string>(
            (x, y) => x.Equals(y, StringComparison.OrdinalIgnoreCase),
            x => x.GetHashCode(StringComparison.OrdinalIgnoreCase));

    private readonly ILocalStorageService _storage;

    private ClaimsPrincipal? _principal;

    public LocalStorageIdentityManager(ILocalStorageService storage)
    {
        _storage = storage;
    }

    public async ValueTask<UserIdentity?> FindIdentityAsync(CancellationToken cancellationToken)
    {
        UserIdentity? identity = await _storage.GetItemAsync<UserIdentity>(IdentityKey, cancellationToken);

        if (identity is null)
        {
            _principal = null;
            return null;
        }

        if (identity.ExpirationDateTime > DateTime.UtcNow)
        {
            UpdatePrincipal(identity);
            return identity;
        }

        await _storage.RemoveItemAsync(IdentityKey, cancellationToken);
        _principal = null;

        return null;
    }

    public ValueTask UpdateIdentityAsync(UserIdentity userIdentity, CancellationToken cancellationToken)
    {
        UpdatePrincipal(userIdentity);
        return _storage.SetItemAsync(IdentityKey, userIdentity, cancellationToken);
    }

    public ValueTask RemoveIdentityAsync(CancellationToken cancellationToken)
    {
        _principal = null;
        return _storage.RemoveItemAsync(IdentityKey, cancellationToken);
    }

    public async ValueTask<bool> HasIdentityAsync(CancellationToken cancellationToken)
    {
        UserIdentity? identity = await _storage.GetItemAsync<UserIdentity>(IdentityKey, cancellationToken);

        if (identity is null)
        {
            _principal = null;
            return false;
        }

        DateTime now = DateTime.UtcNow;

        return identity.ExpirationDateTime > now;
    }

    public bool HasRoles(params string[] roles)
    {
        bool? hasRole = _principal?.Claims
            .Where(x => x.Type.Equals(ClaimTypes.Role, StringComparison.OrdinalIgnoreCase))
            .Join(roles, x => x.Value, x => x, (_, _) => true, OrdinalIgnoreCaseComparer)
            .Any();

        return hasRole is true;
    }

    private void UpdatePrincipal(UserIdentity identity)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken token = tokenHandler.ReadJwtToken(identity.Token);

        _principal = new ClaimsPrincipal(new ClaimsIdentity(token.Claims));
    }
}