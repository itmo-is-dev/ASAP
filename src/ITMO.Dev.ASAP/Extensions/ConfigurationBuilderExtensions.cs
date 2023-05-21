using ITMO.Dev.ASAP.Configuration;

namespace ITMO.Dev.ASAP.Extensions;

internal static class ConfigurationBuilderExtensions
{
    internal static IConfigurationBuilder AddSecretsFromLockBox(this IConfigurationBuilder builder, LockBoxEntry[]? lockBoxEntries)
    {
        LockBoxEntry[] entries = lockBoxEntries ?? Array.Empty<LockBoxEntry>();
        return builder.Add(new LockBoxSecretsConfigurationSource(entries));
    }
}