namespace ITMO.Dev.ASAP.Github.Presentation.Webhooks.Exceptions;

public class GithubWebhookException : Exception
{
    public GithubWebhookException(string? message) : base(message) { }
}