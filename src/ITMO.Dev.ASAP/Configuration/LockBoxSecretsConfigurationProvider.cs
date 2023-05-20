namespace ITMO.Dev.ASAP.Configuration;

internal class LockBoxSecretsConfigurationProvider : ConfigurationProvider
{
    public LockBoxSecretsConfigurationProvider(IEnumerable<LockBoxEntry> entries)
    {
        var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        foreach (LockBoxEntry lockBoxEntry in entries)
        {
            data[Normalize(lockBoxEntry.Key)] = lockBoxEntry.Value;
        }

        Data = data;
    }

    private static string Normalize(string key)
    {
        return key.Replace("__", ConfigurationPath.KeyDelimiter, StringComparison.Ordinal);
    }
}
