using ITMO.Dev.ASAP.Configuration;
using ITMO.Dev.ASAP.Seeding.Extensions;
using IsuUserAssociation = ITMO.Dev.ASAP.Domain.UserAssociations.IsuUserAssociation;
using Mentor = ITMO.Dev.ASAP.Domain.Users.Mentor;
using Student = ITMO.Dev.ASAP.Domain.Users.Student;
using SubjectCourse = ITMO.Dev.ASAP.Domain.Study.SubjectCourse;
using SubjectCourseAssociation = ITMO.Dev.ASAP.Domain.SubjectCourseAssociations.SubjectCourseAssociation;
using Submission = ITMO.Dev.ASAP.Domain.Submissions.Submission;
using User = ITMO.Dev.ASAP.Domain.Users.User;

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
                options.ConfigureEntityGenerator<SubjectCourseAssociation>(o => o.Count = 0);
            })
            .AddDatabaseSeeders();
    }
}