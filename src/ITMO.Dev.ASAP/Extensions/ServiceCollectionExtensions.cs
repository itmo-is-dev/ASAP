using FluentSerialization.Extensions.NewtonsoftJson;
using ITMO.Dev.ASAP.Application.Dto.Tools;
using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.Application.Handlers.Extensions;
using ITMO.Dev.ASAP.Configuration;
using ITMO.Dev.ASAP.Controllers;
using ITMO.Dev.ASAP.DataAccess.Extensions;
using ITMO.Dev.ASAP.Github;
using ITMO.Dev.ASAP.Google;
using ITMO.Dev.ASAP.Identity.Extensions;
using ITMO.Dev.ASAP.Presentation.Rpc.Extensions;
using ITMO.Dev.ASAP.Presentation.Services.Extensions;
using Microsoft.EntityFrameworkCore;
using ConfigurationBuilder = FluentSerialization.ConfigurationBuilder;

namespace ITMO.Dev.ASAP.Extensions;

#pragma warning disable CA1506

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection ConfigureServiceCollection(
        this IServiceCollection serviceCollection,
        IConfiguration configuration,
        WebApiConfiguration webApiConfiguration,
        IConfigurationSection identityConfigurationSection,
        bool isDevelopmentEnvironment)
    {
        serviceCollection
            .AddControllers()
            .AddNewtonsoftJson(x => ConfigurationBuilder
                .Build(new DtoSerializationConfiguration())
                .ApplyToSerializationSettings(x.SerializerSettings))
            .AddApplicationPart(typeof(IControllerProjectMarker).Assembly)
            .AddControllersAsServices();

        serviceCollection.AddRpcPresentation();

        string applicationDatabaseConnectionString = webApiConfiguration.PostgresConfiguration
            .ToConnectionString(webApiConfiguration.DbNamesConfiguration.ApplicationDbName);

        serviceCollection
            .AddSwagger()
            .AddApplicationConfiguration()
            .AddAsapPresentationServices()
            .AddHandlers(configuration)
            .AddDatabaseContext(o => o
                .UseNpgsql(applicationDatabaseConnectionString)
                .UseLazyLoadingProxies());

        serviceCollection.AddIdentityConfiguration(
            identityConfigurationSection,
            x => x.UseNpgsql(
                webApiConfiguration.PostgresConfiguration.ToConnectionString(webApiConfiguration.DbNamesConfiguration
                    .IdentityDbName)));

        serviceCollection.AddAsapGithub(configuration, applicationDatabaseConnectionString);
        serviceCollection.AddAsapGoogle(configuration, applicationDatabaseConnectionString);

        if (isDevelopmentEnvironment && webApiConfiguration.TestEnvironmentConfiguration is not null)
            serviceCollection.AddEntityGeneratorsAndSeeding(webApiConfiguration.TestEnvironmentConfiguration);

        serviceCollection.AddRazorPages();

        return serviceCollection;
    }
}