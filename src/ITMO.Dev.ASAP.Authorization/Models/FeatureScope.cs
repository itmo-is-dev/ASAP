namespace ITMO.Dev.ASAP.Authorization.Models;

public class FeatureScope : Dictionary<string, FeatureRoles>
{
    public FeatureScope() : base(StringComparer.OrdinalIgnoreCase) { }
}