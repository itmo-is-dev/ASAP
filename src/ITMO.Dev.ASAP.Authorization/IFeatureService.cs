using System.Security.Claims;

namespace ITMO.Dev.ASAP.Authorization;

public interface IFeatureService
{
    bool HasFeature(ClaimsPrincipal principal, string scope, string feature);
}