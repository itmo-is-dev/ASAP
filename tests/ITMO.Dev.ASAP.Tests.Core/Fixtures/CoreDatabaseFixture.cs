using ITMO.Dev.ASAP.DataAccess.Context;
using ITMO.Dev.ASAP.DataAccess.Extensions;
using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Domain.Submissions;
using ITMO.Dev.ASAP.Domain.Users;
using ITMO.Dev.ASAP.Seeding.Extensions;
using ITMO.Dev.ASAP.Seeding.Options;
using ITMO.Dev.ASAP.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Respawn.Graph;
using System.Data.Common;

namespace ITMO.Dev.ASAP.Tests.Core.Fixtures;

public class CoreDatabaseFixture : DatabaseFixture
{
    protected override void ConfigureServices(IServiceCollection collection)
    {
        collection.AddDatabaseContext(x =>
            x.UseLazyLoadingProxies().UseNpgsql(Container.GetConnectionString()));

        collection.AddEntityGenerators(x =>
        {
            x.ConfigureEntityGenerator<Submission>(xx => xx.Count = 1000);
            x.ConfigureEntityGenerator<SubjectCourse>(xx => xx.Count = 1);

            x.ConfigureEntityGenerator<Student>(xx => xx.Count = 50);
            x.ConfigureEntityGenerator<User>(xx => xx.Count = 100);

            ConfigureSeeding(x);
        });

        collection.AddDatabaseSeeders();
    }

    public DatabaseContext Context { get; private set; } = null!;

    public AsyncServiceScope Scope { get; private set; } = default;

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