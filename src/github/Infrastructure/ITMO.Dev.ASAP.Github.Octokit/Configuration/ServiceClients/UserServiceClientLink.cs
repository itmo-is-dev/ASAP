using FluentChaining;
using ITMO.Dev.ASAP.Github.Octokit.Clients.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Octokit;

namespace ITMO.Dev.ASAP.Github.Octokit.Configuration.ServiceClients;

public class UserServiceClientLink : ILink<ServiceClientCommand>
{
    public Unit Process(
        ServiceClientCommand request,
        SynchronousContext context,
        LinkDelegate<ServiceClientCommand, SynchronousContext, Unit> next)
    {
        const string sectionPath = "Github:Octokit:Service:User";
        const string enabledPath = $"{sectionPath}:Enabled";
        const string namePath = $"{sectionPath}:Name";

        bool enabled = request.Configuration.GetSection(enabledPath).Get<bool>();

        if (enabled is false)
            return next(request, context);

        string? name = request.Configuration.GetSection(namePath).Get<string>();

        if (name is null)
            return next(request, context);

        request.ServiceCollection.AddSingleton<IServiceClientStrategy>(p =>
        {
            IGitHubClient client = p.GetRequiredService<IGitHubClient>();
            return new UserServiceClientStrategy(client, name);
        });

        return Unit.Value;
    }
}