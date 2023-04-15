using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Contracts.Identity.Commands;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models;
using MediatR;

namespace ITMO.Dev.ASAP.Helpers;

internal static class SeedingHelper
{
    internal static async Task SeedAdmins(IServiceProvider provider, IConfiguration configuration)
    {
        IMediator mediatr = provider.GetRequiredService<IMediator>();
        ILogger<Program> logger = provider.GetRequiredService<ILogger<Program>>();
        IConfigurationSection adminsSection = configuration.GetSection("Identity:DefaultAdmins");
        AdminModel[] admins = adminsSection.Get<AdminModel[]>() ?? Array.Empty<AdminModel>();

        foreach (AdminModel admin in admins)
        {
            try
            {
                var registerCommand = new CreateAdmin.Command(admin.Username, admin.Password);
                await mediatr.Send(registerCommand);

                var promoteCommand = new ChangeUserRole.Command(admin.Username, AsapIdentityRoleNames.AdminRoleName);
                await mediatr.Send(promoteCommand);
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Failed to register admin {AdminUsername}", admin.Username);
            }
        }
    }
}