using ITMO.Dev.ASAP.Presentation.Contracts.Services;
using ITMO.Dev.ASAP.Presentation.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Presentation.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAsapPresentationContracts(this IServiceCollection collection)
    {
        collection.AddScoped<IAsapSubjectCourseService, AsapSubjectCourseService>();
        collection.AddScoped<IAsapUserService, AsapUserService>();

        return collection;
    }
}