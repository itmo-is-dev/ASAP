using ITMO.Dev.ASAP.DataAccess.Configuration;
using ITMO.Dev.ASAP.Integration.Google.Models;

namespace ITMO.Dev.ASAP.WebApi.Configuration;

internal class WebApiConfiguration
{
    public WebApiConfiguration(IConfiguration configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        GoogleIntegrationConfiguration = configuration
            .GetSection(nameof(GoogleIntegrationConfiguration))
            .Get<GoogleIntegrationConfiguration>();

        PostgresConfiguration = configuration.GetSection(nameof(PostgresConfiguration)).Get<PostgresConfiguration>();
        DbNamesConfiguration = configuration.GetSection(nameof(DbNamesConfiguration)).Get<DbNamesConfiguration>();

        TestEnvironmentConfiguration = configuration
            .GetSection(nameof(TestEnvironmentConfiguration))
            .Get<TestEnvironmentConfiguration>();
    }

    public GoogleIntegrationConfiguration GoogleIntegrationConfiguration { get; }

    public PostgresConfiguration PostgresConfiguration { get; }

    public DbNamesConfiguration DbNamesConfiguration { get; }

    public TestEnvironmentConfiguration? TestEnvironmentConfiguration { get; }
}