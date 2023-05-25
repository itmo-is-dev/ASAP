namespace ITMO.Dev.ASAP.Authorization.Models;

internal class FeatureScopes : Dictionary<string, FeatureScope>
{
    public FeatureScopes() : base(StringComparer.OrdinalIgnoreCase) { }
}
