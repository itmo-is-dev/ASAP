using GitHubJwt;

namespace ITMO.Dev.ASAP.Github.Application.Octokit.Configurations;

public class FullStringPrivateKeySource : IPrivateKeySource
{
    private readonly string _key;

    public FullStringPrivateKeySource(string key)
    {
        _key = !string.IsNullOrEmpty(key) ? key : throw new ArgumentNullException(nameof(key));
    }

    public TextReader GetPrivateKeyReader()
    {
        return new StringReader(_key);
    }
}