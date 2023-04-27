using ITMO.Dev.ASAP.Github.Application.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Github.DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubDatabaseContext(
        this IServiceCollection collection,
        Action<DbContextOptionsBuilder> action)
    {
        collection.AddDbContext<IDatabaseContext, GithubDatabaseContext>(action);

        return collection;
    }

    public static Task UseGithubDatabaseContext(this IServiceProvider provider)
    {
        GithubDatabaseContext context = provider.GetRequiredService<GithubDatabaseContext>();
        return context.Database.MigrateAsync();
    }
}