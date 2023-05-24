namespace ITMO.Dev.ASAP.Authorization.Models;

internal class FeatureScope : Dictionary<string, FeatureRoles>
{
    public FeatureScope() : base(StringComparer.OrdinalIgnoreCase) { }
}