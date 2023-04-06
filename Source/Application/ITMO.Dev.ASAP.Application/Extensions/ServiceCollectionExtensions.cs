using FluentChaining;
using FluentChaining.Configurators;
using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Abstractions.Permissions;
using ITMO.Dev.ASAP.Application.Abstractions.SubjectCourses;
using ITMO.Dev.ASAP.Application.Abstractions.Submissions;
using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Application.Queries;
using ITMO.Dev.ASAP.Application.Queries.Adapters;
using ITMO.Dev.ASAP.Application.Queries.Requests;
using ITMO.Dev.ASAP.Application.Services;
using ITMO.Dev.ASAP.Application.Tools;
using ITMO.Dev.ASAP.Application.Users;
using ITMO.Dev.ASAP.Application.Validators;
using ITMO.Dev.ASAP.Core.Queue;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.Users;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationConfiguration(this IServiceCollection collection)
    {
        collection.AddSingleton<IQueryExecutor, QueryExecutor>();
        collection.AddScoped<IPermissionValidator, PermissionValidator>();
        collection.AddScoped<ISubjectCourseService, SubjectCourseService>();
        collection.AddScoped<ISubmissionWorkflowService, SubmissionWorkflowService>();

        collection.AddQueryChains();
        collection.AddFilterChains();

        collection.AddCurrentUser();

        return collection;
    }

    private static void AddCurrentUser(this IServiceCollection collection)
    {
        collection.AddScoped<CurrentUserProxy>();
        collection.AddScoped<ICurrentUser>(x => x.GetRequiredService<CurrentUserProxy>());
        collection.AddScoped<ICurrentUserManager>(x => x.GetRequiredService<CurrentUserProxy>());
    }

    private static void AddQueryChains(this IServiceCollection collection)
    {
        collection.AddEntityQuery<Student, StudentQueryParameter>();
        collection.AddEntityQuery<StudentGroup, GroupQueryParameter>();
        collection.AddEntityQuery<User, UserQueryParameter>();

        collection
            .AddFluentChaining(x => x.ChainLifetime = ServiceLifetime.Singleton)
            .AddQueryChain<Student, StudentQueryParameter>()
            .AddQueryChain<StudentGroup, GroupQueryParameter>()
            .AddQueryChain<User, UserQueryParameter>();
    }

    private static IChainConfigurator AddQueryChain<TValue, TParameter>(this IChainConfigurator configurator)
    {
        return configurator.AddChain<EntityQueryRequest<TValue, TParameter>, IQueryable<TValue>>(x => x
            .ThenFromAssemblies(typeof(IAssemblyMarker))
            .FinishWith((r, _) => r.Query));
    }

    private static void AddEntityQuery<TValue, TParameter>(this IServiceCollection collection)
    {
        collection.AddSingleton<IEntityQuery<TValue, TParameter>, EntityQueryAdapter<TValue, TParameter>>();
    }

    private static void AddFilterChains(this IServiceCollection collection)
    {
        collection.AddEntityFilter<StudentDto, StudentQueryParameter>();

        collection
            .AddFluentChaining(x => x.ChainLifetime = ServiceLifetime.Singleton)
            .AddFilterChain<StudentDto, StudentQueryParameter>();
    }

    private static IChainConfigurator AddFilterChain<TValue, TParameter>(this IChainConfigurator configurator)
    {
        return configurator.AddChain<EntityFilterRequest<TValue, TParameter>, IEnumerable<TValue>>(x => x
            .ThenFromAssemblies(typeof(IAssemblyMarker))
            .FinishWith((r, _) => r.Data));
    }

    private static void AddEntityFilter<TValue, TParameter>(this IServiceCollection collection)
    {
        collection.AddSingleton<IEntityFilter<TValue, TParameter>, EntityFilterAdapter<TValue, TParameter>>();
    }
}