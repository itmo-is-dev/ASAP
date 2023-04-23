using ITMO.Dev.ASAP.Application.Contracts.Tools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Application.Handlers.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection collection, IConfiguration configuration)
    {
        IConfigurationSection paginationSection = configuration.GetSection("Pagination");
        collection.Configure<PaginationConfiguration>(paginationSection);

        collection.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(typeof(IAssemblyMarker)));

        return collection;
    }
}