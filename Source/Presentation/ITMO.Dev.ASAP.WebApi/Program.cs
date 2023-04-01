using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.DataAccess.Extensions;
using ITMO.Dev.ASAP.DeveloperEnvironment;
using ITMO.Dev.ASAP.Identity.Abstractions.Entities;
using ITMO.Dev.ASAP.Identity.Extensions;
using ITMO.Dev.ASAP.WebApi.Configuration;
using ITMO.Dev.ASAP.WebApi.Extensions;
using ITMO.Dev.ASAP.WebApi.Helpers;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ITMO.Dev.ASAP.WebApi;

internal class Program
{
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Host.UseSerilogForAppLogs(builder.Configuration);

        builder.AddDeveloperEnvironment();

        var webApiConfiguration = new WebApiConfiguration(builder.Configuration);
        IConfigurationSection identityConfigurationSection =
            builder.Configuration.GetSection("Identity").GetSection("IdentityConfiguration");

        builder.Services.ConfigureServiceCollection(
            builder.Configuration,
            webApiConfiguration,
            identityConfigurationSection,
            builder.Environment.IsDevelopment());

        WebApplication app = builder.Build().Configure(webApiConfiguration.GithubIntegrationConfiguration);

        using (IServiceScope scope = app.Services.CreateScope())
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, AsapIdentityRole.AdminRoleName),
                new Claim(ClaimTypes.NameIdentifier, Guid.Empty.ToString()),
            }));

            ICurrentUserManager currentUserManager = scope.ServiceProvider.GetRequiredService<ICurrentUserManager>();
            currentUserManager.Authenticate(principal);

            RoleManager<AsapIdentityRole> roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<AsapIdentityRole>>();

            await roleManager.CreateRoleIfNotExistsAsync(AsapIdentityRole.AdminRoleName);
            await roleManager.CreateRoleIfNotExistsAsync(AsapIdentityRole.MentorRoleName);
            await roleManager.CreateRoleIfNotExistsAsync(AsapIdentityRole.ModeratorRoleName);

            await scope.ServiceProvider.UseDatabaseContext();

            await SeedingHelper.SeedAdmins(scope.ServiceProvider, app.Configuration);
        }

        await app.RunAsync();
    }
}