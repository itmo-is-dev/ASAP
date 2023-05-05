using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Configuration;
using ITMO.Dev.ASAP.DataAccess.Extensions;
using ITMO.Dev.ASAP.DeveloperEnvironment;
using ITMO.Dev.ASAP.Extensions;
using ITMO.Dev.ASAP.Github;
using ITMO.Dev.ASAP.Helpers;
using System.Security.Claims;

namespace ITMO.Dev.ASAP;

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
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, AsapIdentityRoleNames.AdminRoleName),
                new Claim(ClaimTypes.NameIdentifier, Guid.Empty.ToString()),
            }));

            ICurrentUserManager currentUserManager = scope.ServiceProvider.GetRequiredService<ICurrentUserManager>();
            currentUserManager.Authenticate(principal);

            IAuthorizationService authorizationService = scope.ServiceProvider
                .GetRequiredService<IAuthorizationService>();

            await authorizationService.CreateRoleIfNotExistsAsync(AsapIdentityRoleNames.AdminRoleName);
            await authorizationService.CreateRoleIfNotExistsAsync(AsapIdentityRoleNames.MentorRoleName);
            await authorizationService.CreateRoleIfNotExistsAsync(AsapIdentityRoleNames.ModeratorRoleName);

            await scope.UseAsapGithubAsync();
            await scope.ServiceProvider.UseDatabaseContext();

            await SeedingHelper.SeedAdmins(scope.ServiceProvider, app.Configuration);
        }

        await app.RunAsync();
    }
}