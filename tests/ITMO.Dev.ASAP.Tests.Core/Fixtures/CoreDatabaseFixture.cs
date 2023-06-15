using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.DataAccess.Extensions;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.DataAccess.Models.Users;
using ITMO.Dev.ASAP.Seeding.Extensions;
using ITMO.Dev.ASAP.Seeding.Options;
using ITMO.Dev.ASAP.Tests.Fixtures;
using ITMO.Dev.ASAP.Tests.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Respawn;
using Respawn.Graph;
using Serilog;

namespace ITMO.Dev.ASAP.Tests.Core.Fixtures;

public class CoreDatabaseFixture : DatabaseFixture
{
    public AsyncServiceScope CreateAsyncScope()
    {
        return Provider.CreateAsyncScope();
    }

    protected override void ConfigureServices(IServiceCollection collection)
    {
        collection.AddDatabaseContext(x => x
            .UseNpgsql(Container.GetConnectionString() + ";Include Error Detail = true;")
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .UseLoggerFactory(LoggerFactory.Create(b => b.AddSerilog(new StaticLogger()).AddConsole())));

        collection.AddDbContext<TestDatabaseContext>(builder => builder
            .UseNpgsql(Container.GetConnectionString() + ";Include Error Detail = true;")
            .UseLazyLoadingProxies()
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .UseLoggerFactory(LoggerFactory.Create(b => b.AddSerilog(new StaticLogger()).AddConsole()))
            .ConfigureWarnings(x => x.Ignore(CoreEventId.NavigationBaseIncludeIgnored)));

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

    protected virtual void ConfigureSeeding(EntityGenerationOptions options) { }

    protected override async ValueTask UseProviderAsync(IServiceProvider provider)
    {
        await using AsyncServiceScope asyncScope = provider.CreateAsyncScope();
        await asyncScope.UseDatabaseContext();
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
}