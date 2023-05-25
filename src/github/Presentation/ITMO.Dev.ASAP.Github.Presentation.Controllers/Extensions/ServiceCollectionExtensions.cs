namespace ITMO.Dev.ASAP.Github.Presentation.Controllers.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubPresentationControllers(this IServiceCollection collection)
    {
        collection.AddControllers().AddApplicationPart(typeof(IGithubControllerMarker).Assembly);
        return collection;
    }
}