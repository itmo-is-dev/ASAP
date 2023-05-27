using ITMO.Dev.ASAP.Extensions.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using System.Data;
using System.Data.Common;
using Testcontainers.PostgreSql;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Fixtures;

public abstract class DatabaseFixture : IAsyncLifetime
{
    private const string User = "postgres";
    private const string Password = "postgres";
    private const string Database = "postgres";

    private Respawner _respawn;

    protected DatabaseFixture()
    {
        Container = new PostgreSqlBuilder()
            .WithUsername(User)
            .WithPassword(Password)
            .WithDatabase(Database)
            .Build();

        Connection = null!;
        Provider = null!;
        _respawn = null!;
    }

    public DbConnection Connection { get; private set; }

    protected PostgreSqlContainer Container { get; }

    protected ServiceProvider Provider { get; private set; }

    public virtual async Task ResetAsync()
    {
        bool wasOpen = Connection.State is ConnectionState.Open;

        if (wasOpen is false)
        {
            await Connection.OpenAsync();
        }

        await _respawn.ResetAsync(Connection);

        if (wasOpen is false)
        {
            await Connection.CloseAsync();
        }
    }

    public async Task InitializeAsync()
    {
        await Container.StartAsync();

        var collection = new ServiceCollection();
        ConfigureServices(collection);

        Provider = collection.BuildServiceProvider();
        await UseProviderAsync(Provider);

        Connection = CreateConnection();
        RespawnerOptions options = GetRespawnOptions();

        bool opened = await Connection.TryOpenAsync(default);

        _respawn = await Respawner.CreateAsync(Connection, options);

        if (opened)
        {
            await Connection.CloseAsync();
        }
    }

    public virtual async Task DisposeAsync()
    {
        await Connection.DisposeAsync();
        await Container.DisposeAsync();
        await Provider.DisposeAsync();
    }

    protected virtual void ConfigureServices(IServiceCollection collection) { }

    protected virtual ValueTask UseProviderAsync(IServiceProvider provider)
    {
        return ValueTask.CompletedTask;
    }

    protected virtual RespawnerOptions GetRespawnOptions()
    {
        return new RespawnerOptions
        {
            SchemasToInclude = new[] { "public" },
            DbAdapter = DbAdapter.Postgres,
        };
    }

    protected virtual DbConnection CreateConnection()
    {
        return new NpgsqlConnection(Container.GetConnectionString());
    }
}