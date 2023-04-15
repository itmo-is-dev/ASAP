using Blazored.LocalStorage;
using ITMO.Dev.ASAP.WebApi.Sdk.Identity;
using ITMO.Dev.ASAP.WebUI.Abstractions.Authorization;
using ITMO.Dev.ASAP.WebUI.Abstractions.ExceptionHandling;
using ITMO.Dev.ASAP.WebUI.Abstractions.Navigation;
using ITMO.Dev.ASAP.WebUI.Abstractions.StudyNavigation;
using ITMO.Dev.ASAP.WebUI.Application.Authorization;
using ITMO.Dev.ASAP.WebUI.Application.ExceptionHandling;
using ITMO.Dev.ASAP.WebUI.Application.Navigation;
using ITMO.Dev.ASAP.WebUI.Application.StudyNavigation;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ITMO.Dev.ASAP.WebUI.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebUiApplication(this IServiceCollection collection)
    {
        collection.AddBlazoredLocalStorage();

        collection.AddCurrentUser();
        collection.AddAuthorization();

        collection.AddErrorHandling();

        collection.AddScoped<INavigationService, NavigationService>();

        collection.AddSingleton<ISubjectProvider, SubjectProvider>();
        collection.AddSingleton<ISubjectCourseProvider, SubjectCourseProvider>();

        return collection;
    }

    private static void AddErrorHandling(this IServiceCollection collection)
    {
        collection.AddScoped<ExceptionManager>();
        collection.AddScoped<IExceptionSink>(x => x.GetRequiredService<ExceptionManager>());
        collection.AddScoped<IExceptionStore>(x => x.GetRequiredService<ExceptionManager>());
        collection.AddScoped<ISafeExecutor, SafeExecutor>();
    }

    private static void AddCurrentUser(this IServiceCollection collection)
    {
        collection.AddSingleton<CurrentUser>();
        collection.AddScoped<ICurrentUser>(x => x.GetRequiredService<CurrentUser>());

        collection.AddScoped<CurrentUser.Consumer>();

        collection.TryAddEnumerable(ServiceDescriptor.Scoped<IPrincipalConsumer, CurrentUser.Consumer>(
            x => x.GetRequiredService<CurrentUser.Consumer>()));
    }

    private static void AddAuthorization(this IServiceCollection collection)
    {
        collection.AddOptions();
        collection.AddAuthorizationCore();

        collection.TryAddEnumerable(ServiceDescriptor.Scoped<ITokenConsumer, LocalStorageTokenConsumer>());
        collection.AddPrincipalTokenConsumer();
        collection.AddTokenProviderConsumer();
        collection.AddStateProviderConsumer();

        collection.AddScoped<IPrincipalService, PrincipalService>();
        collection.AddScoped<IAuthorizationLoader, LocalStorageAuthorizationLoader>();
    }

    private static void AddPrincipalTokenConsumer(this IServiceCollection collection)
    {
        collection.AddSingleton<PrincipalTokenConsumer.Storage>();
        collection.AddScoped<PrincipalTokenConsumer>();

        collection.TryAddEnumerable(ServiceDescriptor.Scoped<INavigationConsumer, PrincipalTokenConsumer>(
            x => x.GetRequiredService<PrincipalTokenConsumer>()));

        collection.TryAddEnumerable(ServiceDescriptor.Scoped<ITokenConsumer, PrincipalTokenConsumer>(
            x => x.GetRequiredService<PrincipalTokenConsumer>()));
    }

    private static void AddTokenProviderConsumer(this IServiceCollection collection)
    {
        collection.AddSingleton<TokenProviderConsumer.Storage>();
        collection.AddScoped<TokenProviderConsumer>();
        collection.AddScoped<ITokenProvider>(x => x.GetRequiredService<TokenProviderConsumer>());

        collection.TryAddEnumerable(ServiceDescriptor.Scoped<ITokenConsumer, TokenProviderConsumer>(
            x => x.GetRequiredService<TokenProviderConsumer>()));
    }

    private static void AddStateProviderConsumer(this IServiceCollection collection)
    {
        collection.AddSingleton<StateProviderPrincipalConsumer>();
        collection.AddScoped<AuthenticationStateProvider>(x => x.GetRequiredService<StateProviderPrincipalConsumer>());

        collection.TryAddEnumerable(ServiceDescriptor.Scoped<IPrincipalConsumer, StateProviderPrincipalConsumer>(
            x => x.GetRequiredService<StateProviderPrincipalConsumer>()));
    }
}