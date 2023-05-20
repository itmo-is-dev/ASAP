namespace ITMO.Dev.ASAP.Configuration;

public class LockBoxSecretsConfigurationSource : IConfigurationSource
{
    private readonly LockBoxEntry[] _entries;

    public LockBoxSecretsConfigurationSource(LockBoxEntry[] entries)
    {
        _entries = entries;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new LockBoxSecretsConfigurationProvider(_entries);
    }
}