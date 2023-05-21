namespace ITMO.Dev.ASAP.Github.Octokit.Clients.Service;

public class InstallationServiceClientStrategy : IServiceClientStrategy
{
    private readonly long _value;

    public InstallationServiceClientStrategy(long value)
    {
        _value = value;
    }

    public ValueTask<long> GetInstallationId()
    {
        return ValueTask.FromResult(_value);
    }
}