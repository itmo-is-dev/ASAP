using Microsoft.Extensions.Caching.Memory;

namespace ITMO.Dev.ASAP.Github.Octokit.Caching;

public interface IAsapMemoryCache : IDisposable
{
    TItem GetOrCreate<TItem>(object key, Func<ICacheEntry, TItem> factory);
}