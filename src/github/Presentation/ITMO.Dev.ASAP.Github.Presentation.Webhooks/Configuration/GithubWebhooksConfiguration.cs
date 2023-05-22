namespace ITMO.Dev.ASAP.Github.Presentation.Webhooks.Configuration;

public class GithubWebhooksConfiguration
{
    public bool Enabled { get; init; }

    public string Secret { get; init; } = string.Empty;
}