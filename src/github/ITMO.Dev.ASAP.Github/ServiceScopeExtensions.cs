using ITMO.Dev.ASAP.Github.DataAccess.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Github;

public static class ServiceScopeExtensions
{
    public static async Task UseAsapGithubAsync(this IServiceScope scope)
    {
        await scope.UseGithubDatabaseContext();
    }
}