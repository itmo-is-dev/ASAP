using Microsoft.AspNetCore.Authorization;

namespace ITMO.Dev.ASAP.Authorization.Tools;

public class AuthorizationFeatureRequirement : IAuthorizationRequirement
{
    public AuthorizationFeatureRequirement(string scope, string feature)
    {
        Scope = scope;
        Feature = feature;
    }

    public string Scope { get; }

    public string Feature { get; }
}