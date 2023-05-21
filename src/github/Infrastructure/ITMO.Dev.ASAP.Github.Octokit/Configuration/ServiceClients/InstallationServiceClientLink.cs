using FluentChaining;
using ITMO.Dev.ASAP.Github.Octokit.Clients.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Github.Octokit.Configuration.ServiceClients;

public class InstallationServiceClientLink : ILink<ServiceClientCommand>
{
    public Unit Process(
        ServiceClientCommand request,
        SynchronousContext context,
        LinkDelegate<ServiceClientCommand, SynchronousContext, Unit> next)
    {
        const string sectionPath = "Github:Octokit:Service:Installation";
        const string enabledPath = $"{sectionPath}:Enabled";
        const string installationIdPath = $"{sectionPath}:Id";

        bool enabled = request.Configuration.GetSection(enabledPath).Get<bool>();

        if (enabled is false)
            return next(request, context);

        long id = request.Configuration.GetSection(installationIdPath).Get<long>();

        request.ServiceCollection.AddSingleton<IServiceClientStrategy>(new InstallationServiceClientStrategy(id));

        return Unit.Value;
    }
}