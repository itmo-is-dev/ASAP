namespace ITMO.Dev.ASAP.Github.Application.Octokit.Configurations;

public class GithubIntegrationConfiguration : IAsapConfiguration
{
    public GithubAuthConfiguration GithubAuthConfiguration { get; set; } = null!;

    public GithubAppConfiguration GithubAppConfiguration { get; set; } = null!;

    public void Verify()
    {
        GithubAuthConfiguration.Verify();
        GithubAppConfiguration.Verify();
    }
}