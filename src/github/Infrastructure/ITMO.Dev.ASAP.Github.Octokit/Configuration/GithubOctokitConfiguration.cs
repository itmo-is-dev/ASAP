using Microsoft.Extensions.Options;
using System.Text;

namespace ITMO.Dev.ASAP.Github.Octokit.Configuration;

public class GithubOctokitConfiguration : IValidateOptions<GithubOctokitConfiguration>
{
    public string PrivateKey { get; init; } = string.Empty;

    public int AppId { get; init; }

    public int JwtExpirationSeconds { get; init; }

    public ValidateOptionsResult Validate(string name, GithubOctokitConfiguration options)
    {
        return Validate(options);
    }

    private static ValidateOptionsResult Validate(GithubOctokitConfiguration options)
    {
        var message = new StringBuilder();

        if (string.IsNullOrEmpty(options.PrivateKey))
        {
            message.AppendLine("Configure Github:Octokit:PrivateKey");
        }

        if (options.AppId is 0)
        {
            message.AppendLine("Configure Github:Octokit:AppId");
        }

        if (message.Length is 0)
            return ValidateOptionsResult.Success;

        return ValidateOptionsResult.Fail(message.ToString());
    }
}