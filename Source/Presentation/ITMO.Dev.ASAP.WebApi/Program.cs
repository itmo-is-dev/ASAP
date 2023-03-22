using ITMO.Dev.ASAP.DataAccess.Extensions;
using ITMO.Dev.ASAP.DeveloperEnvironment;
using ITMO.Dev.ASAP.Github.DataAccess.Extensions;
using ITMO.Dev.ASAP.WebApi.Configuration;
using ITMO.Dev.ASAP.WebApi.Extensions;
using ITMO.Dev.ASAP.WebApi.Helpers;

namespace ITMO.Dev.ASAP.WebApi;

internal class Program
{
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Host.UseSerilogForAppLogs(builder.Configuration);

        builder.AddDeveloperEnvironment();

        var webApiConfiguration = new WebApiConfiguration(builder.Configuration);

        IConfigurationSection identityConfigurationSection = builder.Configuration
            .GetSection("Identity:IdentityConfiguration");

        builder.Services.ConfigureServiceCollection(
            builder.Configuration,
            webApiConfiguration,
            identityConfigurationSection,
            builder.Environment.IsDevelopment() || builder.Environment.IsStaging());

        WebApplication app = builder.Build().Configure();

        using (IServiceScope scope = app.Services.CreateScope())
        {
            await scope.ServiceProvider.UseGithubDatabaseContext();
            await scope.ServiceProvider.UseDatabaseContext();

            await SeedingHelper.SeedAdmins(scope.ServiceProvider, app.Configuration);
        }

        await app.RunAsync();
    }
}