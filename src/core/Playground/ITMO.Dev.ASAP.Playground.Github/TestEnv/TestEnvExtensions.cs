using ITMO.Dev.ASAP.Configuration;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.SubjectCourseAssociations;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.Core.UserAssociations;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Extensions;
using ITMO.Dev.ASAP.Seeding.EntityGenerators;
using ITMO.Dev.ASAP.Seeding.Extensions;
using Microsoft.EntityFrameworkCore;

namespace ITMO.Dev.ASAP.Playground.Github.TestEnv;

public static class TestEnvExtensions
{
    public static IServiceCollection AddGithubPlaygroundDatabase(
        this IServiceCollection serviceCollection,
        TestEnvironmentConfiguration config)
    {
        serviceCollection
            .AddDatabaseContext(
                optionsBuilder => optionsBuilder
                    .UseSqlite("Filename=asap-gh.db")
                    .UseLazyLoadingProxies());

        serviceCollection.AddEntityGenerators(options =>
        {
            options.ConfigureFaker(o => o.Locale = "ru");
            options.ConfigureEntityGenerator<User>(o => o.Count = config.Users.Count);
            options.ConfigureEntityGenerator<Student>(o => o.Count = config.Users.Count);
            options.ConfigureEntityGenerator<IsuUserAssociation>(o => o.Count = 0);
            options.ConfigureEntityGenerator<Submission>(o => o.Count = 0);
            options.ConfigureEntityGenerator<SubjectCourse>(o => o.Count = 1);
            options.ConfigureEntityGenerator<SubjectCourseAssociation>(o => o.Count = 0);
        });

        return serviceCollection.AddDatabaseSeeders();
    }

    public static async Task UseTestEnv(
        this IServiceProvider serviceProvider,
        TestEnvironmentConfiguration config,
        CancellationToken cancellationToken = default)
    {
        return;
#pragma warning disable CS0162
        await serviceProvider.UseDatabaseSeeders(cancellationToken);

        using IServiceScope scope = serviceProvider.CreateScope();

        IDatabaseContext dbContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

        IEntityGenerator<User> userGenerator = serviceProvider.GetRequiredService<IEntityGenerator<User>>();
        IReadOnlyList<User> users = userGenerator.GeneratedEntities;
        dbContext.Users.AttachRange(users);
        for (int index = 0; index < config.Users.Count; index++)
        {
            User user = users[index];
        }

        IEntityGenerator<SubjectCourse> subjectCourseGenerator = serviceProvider
            .GetRequiredService<IEntityGenerator<SubjectCourse>>();

        SubjectCourse subjectCourse = subjectCourseGenerator.GeneratedEntities[0];
        dbContext.SubjectCourses.Attach(subjectCourse);

        await dbContext.SaveChangesAsync(cancellationToken);
#pragma warning restore CS0162
    }
}