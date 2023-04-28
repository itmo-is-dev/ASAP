using ITMO.Dev.ASAP.Configuration;
using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Domain.Submissions;
using ITMO.Dev.ASAP.Domain.UserAssociations;
using ITMO.Dev.ASAP.Domain.Users;
using ITMO.Dev.ASAP.Seeding.Extensions;

namespace ITMO.Dev.ASAP.Extensions;

internal static class TestEnvironmentExtensions
{
    internal static void AddEntityGeneratorsAndSeeding(
        this IServiceCollection serviceCollection,
        TestEnvironmentConfiguration testEnvironmentConfiguration)
    {
        serviceCollection.AddEntityGenerators(options =>
        {
            options.ConfigureFaker(o => o.Locale = "ru");
            options.ConfigureEntityGenerator<User>(o => o.Count = testEnvironmentConfiguration.Users.Count);
            options.ConfigureEntityGenerator<Student>(o => o.Count = testEnvironmentConfiguration.Users.Count);
            options.ConfigureEntityGenerator<Mentor>(o => o.Count = testEnvironmentConfiguration.Users.Count);
            options.ConfigureEntityGenerator<IsuUserAssociation>(o => o.Count = 0);
            options.ConfigureEntityGenerator<Submission>(o => o.Count = 0);
            options.ConfigureEntityGenerator<SubjectCourse>(o => o.Count = 1);
        });

        serviceCollection.AddDatabaseSeeders();
    }
}