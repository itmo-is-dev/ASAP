namespace ITMO.Dev.ASAP.Authorization.Models;

internal class AuthorizationConfiguration
{
    public FeatureScopes FeatureScopes { get; init; } = new FeatureScopes();
}