using ITMO.Dev.ASAP.Github.DataAccess.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Github;

public static class ServiceScopeExtensions
{
    public static async Task UseAsapGithubAsync(this IServiceScope scope)
    {
        IConfiguration configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        if (configuration.GetSection("Github:Enabled").Get<bool>())
        {
            await scope.UseGithubDatabaseContext();
        }
    }
}