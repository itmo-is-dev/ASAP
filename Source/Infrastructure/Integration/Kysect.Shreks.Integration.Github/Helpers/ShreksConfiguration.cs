namespace Kysect.Shreks.Integration.Github.Helpers;

public sealed class ShreksConfiguration : IShreksConfiguration
{
    public CacheConfiguration CacheConfiguration { get; init; }
    public CacheEntryConfiguration CacheEntryConfiguration { get; init; }
    public GithubConfiguration GithubConfiguration { get; init; }

    public ShreksConfiguration AppendSecret(string githubAppSecret)
    {
        GithubConfiguration.SetGithubAppSecret(githubAppSecret);
        return this;
    }

    public void Verify()
    {
        CacheConfiguration.Verify();
        CacheEntryConfiguration.Verify();
        GithubConfiguration.Verify();
    }
}