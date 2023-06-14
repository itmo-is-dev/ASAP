using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.DataAccess.Contexts;
using ITMO.Dev.ASAP.DataAccess.Extensions;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.DataAccess.Models.Users;
using ITMO.Dev.ASAP.Seeding.Extensions;
using ITMO.Dev.ASAP.Seeding.Options;
using ITMO.Dev.ASAP.Tests.Fixtures;
using ITMO.Dev.ASAP.Tests.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Respawn;
using Respawn.Graph;
using Serilog;
using System.Data.Common;

namespace ITMO.Dev.ASAP.Tests.Core.Fixtures;

public class CoreDatabaseFixture : DatabaseFixture
{
    protected override void ConfigureServices(IServiceCollection collection)
    {
        collection.AddDatabaseContext((p, x) => x
            .UseLazyLoadingProxies()
            .UseNpgsql(Container.GetConnectionString())
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .UseLoggerFactory(LoggerFactory.Create(b => b.AddSerilog(new StaticLogger()))));

        collection.AddApplicationConfiguration();

        collection.AddEntityGenerators(x =>
        {
            x.ConfigureEntityGenerator<SubmissionModel>(xx => xx.Count = 1000);
            x.ConfigureEntityGenerator<SubjectCourseModel>(xx => xx.Count = 1);
            x.ConfigureEntityGenerator<StudentModel>(xx => xx.Count = 50);
            x.ConfigureEntityGenerator<UserModel>(xx => xx.Count = 100);

            ConfigureSeeding(x);
        });

        collection.AddDatabaseSeeders();
    }

    public DatabaseContext Context { get; private set; } = null!;

    public IPersistenceContext PersistenceContext { get; private set; } = null!;

    public AsyncServiceScope Scope { get; private set; }

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await Scope.DisposeAsync();
    }

    public override async Task ResetAsync()
    {
        await base.ResetAsync();
        Context.ChangeTracker.Clear();
        await Scope.UseDatabaseSeeders();
    }

    protected virtual void ConfigureSeeding(EntityGenerationOptions options) { }

    protected override async ValueTask UseProviderAsync(IServiceProvider provider)
    {
        Scope = provider.CreateAsyncScope();

        // Caching to local variable to avoid redundant boxing
        IServiceScope scope = Scope;

        await scope.UseDatabaseContext();
        await scope.UseDatabaseSeeders();

        Context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        PersistenceContext = scope.ServiceProvider.GetRequiredService<IPersistenceContext>();
    }

    protected override RespawnerOptions GetRespawnOptions()
    {
        return new RespawnerOptions
        {
            SchemasToInclude = new[] { "public" },
            DbAdapter = DbAdapter.Postgres,
            TablesToIgnore = new[] { new Table("__EFMigrationsHistory") },
        };
    }

    protected override DbConnection CreateConnection()
    {
        return Context.Database.GetDbConnection();
    }
}