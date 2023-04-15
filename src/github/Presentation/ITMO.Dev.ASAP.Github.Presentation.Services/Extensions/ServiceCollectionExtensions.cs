using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using ITMO.Dev.ASAP.Github.Presentation.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Github.Presentation.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubPresentationServices(this IServiceCollection collection)
    {
        collection.AddScoped<IGithubUserService, GithubUserService>();
        collection.AddScoped<IGithubSubjectCourseService, GithubSubjectCourseService>();

        return collection;
    }
}