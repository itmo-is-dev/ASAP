using FluentChaining;
using FluentChaining.Configurators;
using ITMO.Dev.ASAP.Application.Abstractions.Formatters;
using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Abstractions.Permissions;
using ITMO.Dev.ASAP.Application.Abstractions.Queue;
using ITMO.Dev.ASAP.Application.Abstractions.SubjectCourses;
using ITMO.Dev.ASAP.Application.Abstractions.Submissions;
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Application.Queries;
using ITMO.Dev.ASAP.Application.Queries.Adapters;
using ITMO.Dev.ASAP.Application.Queries.Requests;
using ITMO.Dev.ASAP.Application.Queue;
using ITMO.Dev.ASAP.Application.SubjectCourses;
using ITMO.Dev.ASAP.Application.Submissions.Workflows;
using ITMO.Dev.ASAP.Application.Users;
using ITMO.Dev.ASAP.Application.Validators;
using ITMO.Dev.ASAP.Application.Workers;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationConfiguration(this IServiceCollection collection)
    {
        collection.AddScoped<IPermissionValidator, PermissionValidator>();
        collection.AddScoped<ISubjectCourseService, SubjectCourseService>();
        collection.AddScoped<ISubmissionWorkflowService, SubmissionWorkflowService>();
        collection.AddScoped<IQueueService, QueueService>();

        collection.AddSingleton<IUserFullNameFormatter, UserFullNameFormatter>();

        collection.AddQueryChains();
        collection.AddFilterChains();

        collection.AddCurrentUser();

        collection.AddUpdateWorkers();

        return collection;
    }

    private static void AddCurrentUser(this IServiceCollection collection)
    {
        collection.AddScoped<CurrentUserProxy>();
        collection.AddScoped<ICurrentUser>(x => x.GetRequiredService<CurrentUserProxy>());
        collection.AddScoped<ICurrentUserManager>(x => x.GetRequiredService<CurrentUserProxy>());
    }

    private static void AddUpdateWorkers(this IServiceCollection collection)
    {
        collection.AddSingleton<QueueUpdater>();
        collection.AddSingleton<IQueueUpdateService>(x => x.GetRequiredService<QueueUpdater>());

        collection.AddSingleton<SubjectCourseUpdater>();
        collection.AddSingleton<ISubjectCourseUpdateService>(x => x.GetRequiredService<SubjectCourseUpdater>());

        collection.AddHostedService<QueueUpdateWorker>();
        collection.AddHostedService<SubjectCoursePointsUpdateWorker>();
    }

    private static void AddQueryChains(this IServiceCollection collection)
    {
        collection.AddEntityQuery<StudentQuery.Builder, StudentQueryParameter>();
        collection.AddEntityQuery<StudentGroupQuery.Builder, GroupQueryParameter>();
        collection.AddEntityQuery<UserQuery.Builder, UserQueryParameter>();

        collection
            .AddFluentChaining(x => x.ChainLifetime = ServiceLifetime.Singleton)
            .AddQueryChain<StudentQuery.Builder, StudentQueryParameter>()
            .AddQueryChain<StudentGroupQuery.Builder, GroupQueryParameter>()
            .AddQueryChain<UserQuery.Builder, UserQueryParameter>();
    }

    private static IChainConfigurator AddQueryChain<TValue, TParameter>(this IChainConfigurator configurator)
    {
        return configurator.AddChain<EntityQueryRequest<TValue, TParameter>, TValue>(x => x
            .ThenFromAssemblies(typeof(IAssemblyMarker))
            .FinishWith((r, _) => r.QueryBuilder));
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