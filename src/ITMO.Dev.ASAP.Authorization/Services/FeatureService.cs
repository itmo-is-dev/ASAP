using ITMO.Dev.ASAP.Authorization.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace ITMO.Dev.ASAP.Authorization.Services;

internal class FeatureService : IFeatureService
{
    private readonly IOptionsMonitor<AuthorizationConfiguration> _options;
    private readonly ILogger<FeatureService> _logger;

    public FeatureService(IOptionsMonitor<AuthorizationConfiguration> options, ILogger<FeatureService> logger)
    {
        _options = options;
        _logger = logger;
    }

    public bool HasFeature(ClaimsPrincipal principal, string scope, string feature)
    {
        if (principal.Identity?.IsAuthenticated is not true)
            return false;

        FeatureScopes featureScopes = _options.CurrentValue.FeatureScopes;

        if (featureScopes.TryGetValue(scope, out FeatureScope? featureScope) is false)
        {
            _logger.LogWarning("Feature scope = {Scope} not found", scope);
            return false;
        }

        if (featureScope.TryGetValue(feature, out FeatureRoles? featureRoles) is false)
        {
            _logger.LogWarning("Feature = {Feature} for scope = {Scope} not found", feature, scope);
            return false;
        }

        return principal.Claims
            .Where(x => x.Type.Equals(ClaimTypes.Role, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Value)
            .Intersect(featureRoles, StringComparer.OrdinalIgnoreCase)
            .Any();
    }
}