using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using ITMO.Dev.ASAP.WebUI.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.WebUI.Markup;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebUiMarkup(this IServiceCollection collection)
    {
        collection
            .AddBlazorise(options => options.Immediate = true)
            .AddBootstrapProviders()
            .AddFontAwesomeIcons();

        var exceptionDisplayConfiguration = new ExceptionDisplayConfiguration(TimeSpan.FromSeconds(6));
        collection.AddSingleton(exceptionDisplayConfiguration);

        return collection;
    }
}