using ITMO.Dev.ASAP.DataAccess.Configuration;

namespace ITMO.Dev.ASAP.Configuration;

internal class WebApiConfiguration
{
    public WebApiConfiguration(IConfiguration configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

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

    public PostgresConfiguration PostgresConfiguration { get; }

    public DbNamesConfiguration DbNamesConfiguration { get; }

    public TestEnvironmentConfiguration? TestEnvironmentConfiguration { get; }
}