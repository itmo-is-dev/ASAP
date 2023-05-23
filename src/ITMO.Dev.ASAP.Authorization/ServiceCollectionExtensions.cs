using ITMO.Dev.ASAP.Authorization.Models;
using ITMO.Dev.ASAP.Authorization.Services;
using ITMO.Dev.ASAP.Authorization.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Authorization;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFeatureAuthorization(
        this IServiceCollection collection,
        IConfiguration configuration)
    {
        IConfigurationSection section = configuration.GetSection("Authorization");
        collection.Configure<AuthorizationConfiguration>(section);

        collection.AddSingleton<IAuthorizationPolicyProvider, AuthorizationFeaturePolicyProvider>();
        collection.AddSingleton<IAuthorizationHandler, FeatureAuthorizationHandler>();
        collection.AddSingleton<IFeatureService, FeatureService>();

        return collection;
    }
}