using ITMO.Dev.ASAP.Configuration;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.DataAccess.Models.UserAssociations;
using ITMO.Dev.ASAP.DataAccess.Models.Users;
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
            options.ConfigureEntityGenerator<UserModel>(o => o.Count = testEnvironmentConfiguration.Users.Count);
            options.ConfigureEntityGenerator<StudentModel>(o => o.Count = testEnvironmentConfiguration.Users.Count);
            options.ConfigureEntityGenerator<MentorModel>(o => o.Count = testEnvironmentConfiguration.Users.Count);
            options.ConfigureEntityGenerator<IsuUserAssociationModel>(o => o.Count = 0);
            options.ConfigureEntityGenerator<SubmissionModel>(o => o.Count = 0);
            options.ConfigureEntityGenerator<SubjectCourseModel>(o => o.Count = 1);
        });

        serviceCollection.AddDatabaseSeeders();
    }
}