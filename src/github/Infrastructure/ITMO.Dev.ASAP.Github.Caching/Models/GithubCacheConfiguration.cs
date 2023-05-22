namespace ITMO.Dev.ASAP.Github.Caching.Models;

public class GithubCacheConfiguration
{
    public long? SizeLimit { get; init; }

    public int ExpirationScanFrequencySeconds { get; init; }

    public int? EntryAbsoluteExpirationSeconds { get; init; }

    public int? EntrySlidingExpirationSeconds { get; init; }

    public TimeSpan ExpirationScanFrequency => TimeSpan.FromSeconds(ExpirationScanFrequencySeconds);

    public TimeSpan? EntryAbsoluteExpiration => EntryAbsoluteExpirationSeconds is null
        ? null
        : TimeSpan.FromSeconds(EntryAbsoluteExpirationSeconds.Value);

    public TimeSpan? EntrySlidingExpiration => EntrySlidingExpirationSeconds is null
        ? null
        : TimeSpan.FromSeconds(EntrySlidingExpirationSeconds.Value);
}