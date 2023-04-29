using ITMO.Dev.ASAP.Google.Application.Extensions;
using ITMO.Dev.ASAP.Google.Application.Handlers.Extensions;
using ITMO.Dev.ASAP.Google.DataAccess.Extensions;
using ITMO.Dev.ASAP.Google.Presentation.Services.Extensions;
using ITMO.Dev.ASAP.Google.Spreadsheets.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace ITMO.Dev.ASAP.Google;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAsapGoogle(
        this IServiceCollection collection,
        IConfiguration configuration,
        string databaseConnectionString)
    {
        bool enabled = configuration.GetValue<bool>("Google:Enabled");

        if (enabled)
        {
            collection
                .AddGoogleApplication()
                .AddGoogleApplicationHandlers()
                .AddGoogleInfrastructureIntegration(configuration)
                .AddGooglePresentationServices();

            collection.AddGoogleDataAccess(_ => new NpgsqlConnection(databaseConnectionString));
        }
        else
        {
            collection.AddDummyGooglePresentationServices();
        }

        return collection;
    }
}