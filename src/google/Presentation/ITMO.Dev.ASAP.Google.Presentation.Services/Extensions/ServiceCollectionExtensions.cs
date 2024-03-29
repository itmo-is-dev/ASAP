using ITMO.Dev.ASAP.Google.Presentation.Contracts.Services;
using ITMO.Dev.ASAP.Google.Presentation.Services.Dummy;
using ITMO.Dev.ASAP.Google.Presentation.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Google.Presentation.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGooglePresentationServices(this IServiceCollection collection)
    {
        collection.AddScoped<IGoogleSubjectCourseService, GoogleSubjectCourseService>();
        return collection;
    }

    public static IServiceCollection AddDummyGooglePresentationServices(this IServiceCollection collection)
    {
        collection.AddScoped<IGoogleSubjectCourseService, DummyGoogleSubjectCourseService>();
        return collection;
    }
}