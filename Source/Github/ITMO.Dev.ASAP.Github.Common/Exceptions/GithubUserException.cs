namespace ITMO.Dev.ASAP.Github.Common.Exceptions;

public class GithubUserException : AsapGithubException
{
    private GithubUserException(string? message) : base(message) { }

    public static GithubUserException UsernameCollision(string username)
        => new GithubUserException($"User with github {username} already exists");

    public static GithubUserException UserDoesNotExist(string username)
        => new GithubUserException($"User with github {username} does not exist");
}