namespace ITMO.Dev.ASAP.Github.Octokit.Clients.Service;

public interface IServiceClientStrategy
{
    ValueTask<long> GetInstallationId();
}