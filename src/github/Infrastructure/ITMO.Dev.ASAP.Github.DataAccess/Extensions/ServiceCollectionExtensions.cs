using FluentMigrator.Runner;
using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.Github.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace ITMO.Dev.ASAP.Github.DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubDatabaseContext(
        this IServiceCollection collection,
        string databaseConnectionString)
    {
        collection.AddScoped(_ => new GithubDbConnection(new NpgsqlConnection(databaseConnectionString)));

        collection.AddScoped<IUnitOfWork, UnitOfWork>();
        collection.AddScoped<IPersistenceContext, GithubPersistenceContext>();

        collection.AddScoped<IGithubAssignmentRepository, GithubAssignmentRepository>();
        collection.AddScoped<IGithubSubjectCourseRepository, GithubSubjectCourseRepository>();
        collection.AddScoped<IGithubSubmissionRepository, GithubSubmissionRepository>();
        collection.AddScoped<IGithubUserRepository, GithubUserRepository>();

        collection.AddMigrations(_ => databaseConnectionString);

        return collection;
    }

    public static Task UseGithubDatabaseContext(this IServiceScope provider)
    {
        IMigrationRunner runner = provider.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();

        return Task.CompletedTask;
    }

    internal static void AddMigrations(
        this IServiceCollection collection,
        Func<IServiceProvider, string> connectionStringFactory)
    {
        collection.AddFluentMigratorCore()
            .ConfigureRunner(x => x
                .AddPostgres()
                .WithGlobalConnectionString(connectionStringFactory)
                .WithMigrationsIn(typeof(IAssemblyMarker).Assembly));
    }
}