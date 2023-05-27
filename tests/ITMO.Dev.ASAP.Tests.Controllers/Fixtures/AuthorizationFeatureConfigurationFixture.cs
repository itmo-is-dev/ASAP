using ITMO.Dev.ASAP.Authorization;
using ITMO.Dev.ASAP.Authorization.Models;
using ITMO.Dev.ASAP.Exceptions;
using Microsoft.Extensions.Configuration;

namespace ITMO.Dev.ASAP.Tests.Controllers.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public class AuthorizationFeatureConfigurationFixture
{
    internal AuthorizationConfiguration Configuration { get; }

    public AuthorizationFeatureConfigurationFixture()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile("features.json")
            .Build();

        Configuration = configuration.GetSection(Constants.SectionKey).Get<AuthorizationConfiguration>()
                         ?? throw new StartupException("Failed to parse features.json");
    }
}