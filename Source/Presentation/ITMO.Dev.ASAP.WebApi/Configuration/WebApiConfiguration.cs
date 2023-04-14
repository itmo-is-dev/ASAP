using ITMO.Dev.ASAP.DataAccess.Configuration;
using ITMO.Dev.ASAP.Integration.Google.Models;

namespace ITMO.Dev.ASAP.WebApi.Configuration;

internal class WebApiConfiguration
{
    public WebApiConfiguration(IConfiguration configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        GoogleIntegrationConfiguration? googleIntegrationConfiguration = configuration
            .GetSection(nameof(GoogleIntegrationConfiguration))
            .Get<GoogleIntegrationConfiguration>();

        GoogleIntegrationConfiguration = googleIntegrationConfiguration ??
                                         throw new ArgumentException(nameof(GoogleIntegrationConfiguration));

        PostgresConfiguration? postgresConfiguration = configuration
            .GetSection(nameof(PostgresConfiguration))
            .Get<PostgresConfiguration>();

        PostgresConfiguration = postgresConfiguration ??
                                throw new ArgumentException(nameof(PostgresConfiguration));

        DbNamesConfiguration? dbNamesConfiguration = configuration
            .GetSection(nameof(DbNamesConfiguration))
            .Get<DbNamesConfiguration>();

        DbNamesConfiguration = dbNamesConfiguration ??
                               throw new ArgumentException(nameof(DbNamesConfiguration));

        TestEnvironmentConfiguration = configuration
            .GetSection(nameof(TestEnvironmentConfiguration))
            .Get<TestEnvironmentConfiguration>();
    }

    public GoogleIntegrationConfiguration GoogleIntegrationConfiguration { get; }

    public PostgresConfiguration PostgresConfiguration { get; }

    public DbNamesConfiguration DbNamesConfiguration { get; }

    public TestEnvironmentConfiguration? TestEnvironmentConfiguration { get; }
}