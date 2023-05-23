using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace ITMO.Dev.ASAP.Authorization.Tools;

public class AuthorizationFeaturePolicyProvider : IAuthorizationPolicyProvider
{
    private readonly IAuthorizationPolicyProvider _default;

    public AuthorizationFeaturePolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _default = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(AuthorizeFeatureAttribute.Prefix, StringComparison.OrdinalIgnoreCase) is false)
            return _default.GetPolicyAsync(policyName);

        var builder = new AuthorizationPolicyBuilder();

        int start = AuthorizeFeatureAttribute.Prefix.Length;
        int separator = policyName.IndexOf(':', start);

        if (separator < 0)
            return _default.GetPolicyAsync(policyName);

        string scope = policyName[start..separator];
        string feature = policyName[(separator + 1)..];

        builder.AddRequirements(new AuthorizationFeatureRequirement(scope, feature));

        AuthorizationPolicy policy = builder.Build();

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return _default.GetDefaultPolicyAsync();
    }

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
    {
        return _default.GetFallbackPolicyAsync();
    }
}