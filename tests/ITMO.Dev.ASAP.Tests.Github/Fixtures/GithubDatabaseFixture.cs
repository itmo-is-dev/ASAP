using ITMO.Dev.ASAP.Github.DataAccess;
using ITMO.Dev.ASAP.Github.DataAccess.Extensions;
using ITMO.Dev.ASAP.Tests.Fixtures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Tests.Github.Fixtures;

public class GithubDatabaseFixture : DatabaseFixture
{
    public GithubDbConnection GithubConnection => new GithubDbConnection(Connection);

    protected override void ConfigureServices(IServiceCollection collection)
    {
        var configurationValues = new Dictionary<string, string?>
        {
            { "Github:Enabled", "true" },
        };

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationValues)
            .Build();

        collection.AddSingleton<IConfiguration>(configuration);

        collection.AddMigrations(_ => Container.GetConnectionString());
    }

    protected override async ValueTask UseProviderAsync(IServiceProvider provider)
    {
        await using AsyncServiceScope scope = provider.CreateAsyncScope();
        await scope.UseGithubDatabaseContext();
    }
}