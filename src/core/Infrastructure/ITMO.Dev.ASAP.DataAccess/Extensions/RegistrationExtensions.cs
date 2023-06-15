using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.DataAccess.Contexts;
using ITMO.Dev.ASAP.DataAccess.Repositories;
using ITMO.Dev.ASAP.DataAccess.Repositories.Students;
using ITMO.Dev.ASAP.DataAccess.Repositories.SubjectCourses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.DataAccess.Extensions;

public static class RegistrationExtensions
{
    public static IServiceCollection AddDatabaseContext(
        this IServiceCollection collection,
        Action<DbContextOptionsBuilder> action)
    {
        return collection.AddDatabaseContext((_, b) => action.Invoke(b));
    }

    public static IServiceCollection AddDatabaseContext(
        this IServiceCollection collection,
        Action<IServiceProvider, DbContextOptionsBuilder> action)
    {
        collection.AddDbContext<DatabaseContext>((provider, builder) =>
        {
            action.Invoke(provider, builder);
            builder.ConfigureWarnings(x => x.Ignore(CoreEventId.NavigationBaseIncludeIgnored));
        });

        collection.AddScoped<IPersistenceContext, PersistenceContext>();

        collection.AddScoped<IUserRepository, UserRepository>();
        collection.AddScoped<IStudentRepository, StudentRepository>();
        collection.AddScoped<IMentorRepository, MentorRepository>();
        collection.AddScoped<IAssignmentRepository, AssignmentRepository>();
        collection.AddScoped<IGroupAssignmentRepository, GroupAssignmentRepository>();
        collection.AddScoped<IStudentGroupRepository, StudentGroupRepository>();
        collection.AddScoped<ISubjectRepository, SubjectRepository>();
        collection.AddScoped<ISubjectCourseRepository, SubjectCourseRepository>();
        collection.AddScoped<ISubmissionRepository, SubmissionRepository>();
        collection.AddScoped<IUserAssociationRepository, UserAssociationRepository>();
        collection.AddScoped<IStudentAssignmentRepository, StudentAssignmentRepository>();

        return collection;
    }

    public static Task UseDatabaseContext(this IServiceScope scope)
    {
        DatabaseContext context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        return context.Database.MigrateAsync();
    }
}