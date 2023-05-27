using ITMO.Dev.ASAP.Application.Abstractions.Tools;
using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.DataAccess.Context;
using ITMO.Dev.ASAP.DataAccess.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.DataAccess.Extensions;

public static class RegistrationExtensions
{
    public static IServiceCollection AddDatabaseContext(
        this IServiceCollection collection,
        Action<DbContextOptionsBuilder> action)
    {
        collection.AddDbContext<IDatabaseContext, DatabaseContext>(action);
        collection.AddSingleton(typeof(IPatternMatcher<>), typeof(PatternMatcher<>));

        return collection;
    }

    public static Task UseDatabaseContext(this IServiceScope scope)
    {
        DatabaseContext context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        return context.Database.MigrateAsync();
    }
}