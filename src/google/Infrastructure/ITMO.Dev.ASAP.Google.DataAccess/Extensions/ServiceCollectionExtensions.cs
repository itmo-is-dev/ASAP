using ITMO.Dev.ASAP.Google.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.Google.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace ITMO.Dev.ASAP.Google.DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGoogleDataAccess(
        this IServiceCollection collection,
        Func<IServiceProvider, IDbConnection> factory)
    {
        collection.AddScoped(factory);

        collection.AddScoped<ISubjectCourseRepository, SubjectCourseRepository>();

        return collection;
    }
}