using Blazored.LocalStorage;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using ITMO.Dev.ASAP.Application.Dto.Extensions;
using ITMO.Dev.ASAP.WebApi.Sdk.Extensions;
using ITMO.Dev.ASAP.WebApi.Sdk.Identity;
using ITMO.Dev.ASAP.WebUI.AdminPanel.Authorization;
using ITMO.Dev.ASAP.WebUI.AdminPanel.Authorization.Implementations;
using ITMO.Dev.ASAP.WebUI.AdminPanel.ExceptionHandling;
using ITMO.Dev.ASAP.WebUI.AdminPanel.Navigation;
using ITMO.Dev.ASAP.WebUI.AdminPanel.Navigation.Implementations;
using ITMO.Dev.ASAP.WebUI.AdminPanel.Tools;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddAdminPanel(this IServiceCollection collection, IWebAssemblyHostEnvironment environment)
    {
        collection.AddBlazoredLocalStorage();

        collection.AddDtoConfiguration();
        collection.AddSingleton(new EnvironmentConfiguration(environment.IsDevelopment()));

        collection
            .AddBlazorise(options => options.Immediate = true)
            .AddBootstrapProviders()
            .AddFontAwesomeIcons();

        var exceptionDisplayConfiguration = new ExceptionDisplayConfiguration(TimeSpan.FromSeconds(6));
        collection.AddSingleton(exceptionDisplayConfiguration);

        collection.AddScoped<ExceptionManager>();
        collection.AddScoped<IExceptionSink>(x => x.GetRequiredService<ExceptionManager>());
        collection.AddScoped<IExceptionStore>(x => x.GetRequiredService<ExceptionManager>());
        collection.AddScoped<ISafeExecutor, SafeExecutor>();

        collection.AddOptions();
        collection.AddAuthorizationCore();
        collection.AddAsapAuthorization();

        collection.AddScoped<INavigationService, NavigationService>();

        collection.AddAsapSdk(new Uri(environment.BaseAddress));
    }

    public static void AddAsapAuthorization(this IServiceCollection collection)
    {
        collection.TryAddEnumerable(ServiceDescriptor.Scoped<ITokenConsumer, LocalStorageTokenConsumer>());
        collection.AddPrincipalTokenConsumer();
        collection.AddTokenProviderConsumer();
        collection.AddStateProviderConsumer();
        collection.AddCurrentUser();

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

    private static void AddCurrentUser(this IServiceCollection collection)
    {
        collection.AddSingleton<CurrentUser>();
        collection.AddScoped<ICurrentUser>(x => x.GetRequiredService<CurrentUser>());

        collection.AddScoped<CurrentUser.Consumer>();

        collection.TryAddEnumerable(ServiceDescriptor.Scoped<IPrincipalConsumer, CurrentUser.Consumer>(
            x => x.GetRequiredService<CurrentUser.Consumer>()));
    }
}