namespace ITMO.Dev.ASAP.Authorization.Models;

public class FeatureScopes : Dictionary<string, FeatureScope>
{
    public FeatureScopes() : base(StringComparer.OrdinalIgnoreCase) { }
}