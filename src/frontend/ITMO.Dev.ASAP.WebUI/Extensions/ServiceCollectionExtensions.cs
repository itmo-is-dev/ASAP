using ITMO.Dev.ASAP.Application.Dto.Extensions;
using ITMO.Dev.ASAP.WebApi.Sdk.Extensions;
using ITMO.Dev.ASAP.WebUI.Abstractions.Models;
using ITMO.Dev.ASAP.WebUI.Application;
using ITMO.Dev.ASAP.WebUI.Application.ViewModels.Extensions;
using ITMO.Dev.ASAP.WebUI.Markup;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace ITMO.Dev.ASAP.WebUI.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddAdminPanel(this IServiceCollection collection, IWebAssemblyHostEnvironment environment)
    {
        collection.AddDtoConfiguration();
        collection.AddSingleton(new EnvironmentConfiguration(environment.IsDevelopment()));

        collection.AddWebUiApplication();
        collection.AddWebUiMarkup();
        collection.AddViewModels();

        collection.AddAsapSdk(new Uri(environment.BaseAddress));
    }
}