using Bogus;
using FluentScanning;
using FluentScanning.DependencyInjection;
using ITMO.Dev.ASAP.DataAccess.Contexts;
using ITMO.Dev.ASAP.Seeding.DatabaseSeeders;
using ITMO.Dev.ASAP.Seeding.EntityGenerators;
using ITMO.Dev.ASAP.Seeding.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ITMO.Dev.ASAP.Seeding.Extensions;

public static class RegistrationExtensions
{
    public static IServiceCollection AddEntityGenerators(
        this IServiceCollection collection,
        Action<EntityGenerationOptions>? action = null)
    {
        var generationOptions = new EntityGenerationOptions(collection);
        action?.Invoke(generationOptions);

        collection.AddSingleton(typeof(EntityGeneratorOptions<>));
        collection.TryAddSingleton<Faker>();

        using ServiceCollectionAssemblyScanner? scanner = collection.UseAssemblyScanner(typeof(IAssemblyMarker));

        scanner.EnqueueAdditionOfTypesThat()
            .WouldBeRegisteredAsTypesConstructedFrom(typeof(IEntityGenerator<>))
            .WithSingletonLifetime()
            .AreBasedOnTypesConstructedFrom(typeof(IEntityGenerator<>))
            .AreNotAbstractClasses()
            .AreNotInterfaces();

        return collection;
    }

    public static IServiceCollection AddDatabaseSeeders(this IServiceCollection collection)
    {
        using ServiceCollectionAssemblyScanner? scanner = collection.UseAssemblyScanner(typeof(IAssemblyMarker));

        scanner.EnqueueAdditionOfTypesThat()
            .WouldBeRegisteredAs<IDatabaseSeeder>()
            .WithSingletonLifetime()
            .AreAssignableTo<IDatabaseSeeder>()
            .AreNotAbstractClasses()
            .AreNotInterfaces();

        return collection;
    }

    public static async Task UseDatabaseSeeders<TContext>(
        this IServiceScope scope,
        CancellationToken cancellationToken = default) where TContext : DatabaseContext
    {
        DatabaseContext context = scope.ServiceProvider.GetRequiredService<TContext>();

        IEnumerable<IDatabaseSeeder> seeders = scope.ServiceProvider
            .GetServices<IDatabaseSeeder>()
            .OrderByDescending(x => x.Priority);

        foreach (IDatabaseSeeder seeder in seeders)
        {
            seeder.Seed(context);
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}