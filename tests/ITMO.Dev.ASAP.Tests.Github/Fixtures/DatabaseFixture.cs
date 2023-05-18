using ITMO.Dev.ASAP.Github.DataAccess;
using ITMO.Dev.ASAP.Github.DataAccess.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using System.Data;
using System.Data.Common;
using Testcontainers.PostgreSql;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Github.Fixtures;

#pragma warning disable CA1001
public class DatabaseFixture : IAsyncLifetime
{
    private const string User = "postgres";
    private const string Password = "postgres";
    private const string Database = "postgres";

    private readonly PostgreSqlContainer _container;
    private DbConnection _connection;
    private Respawner _respawn;

    public DatabaseFixture()
    {
        _container = new PostgreSqlBuilder()
            .WithUsername(User)
            .WithPassword(Password)
            .WithDatabase(Database)
            .Build();

        _connection = null!;
        _respawn = null!;
    }

    public GithubDbConnection Connection => new GithubDbConnection(_connection);

    public IDbConnection RawConnection => _connection;

    public Task ResetAsync()
    {
        return _respawn.ResetAsync(_connection);
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        _connection = new NpgsqlConnection(_container.GetConnectionString());
        await _connection.OpenAsync();

        var collection = new ServiceCollection();
        collection.AddMigrations(_ => _container.GetConnectionString());

        await using ServiceProvider provider = collection.BuildServiceProvider();
        await using AsyncServiceScope scope = provider.CreateAsyncScope();

        await scope.UseGithubDatabaseContext();

        var options = new RespawnerOptions
        {
            SchemasToInclude = new[]
            {
                "public",
            },
            DbAdapter = DbAdapter.Postgres,
        };

        _respawn = await Respawner.CreateAsync(_connection, options);

        await _connection.CloseAsync();
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
        await _container.DisposeAsync();
    }
}