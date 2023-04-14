using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.SubjectCourseAssociations;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.Core.UserAssociations;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.Seeding.Extensions;
using ITMO.Dev.ASAP.WebApi.Configuration;

namespace ITMO.Dev.ASAP.WebApi.Extensions;

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
                options.ConfigureEntityGenerator<SubjectCourseAssociation>(o => o.Count = 0);
            })
            .AddDatabaseSeeders();
    }
}