namespace ITMO.Dev.ASAP.Github.Common.Exceptions;

public abstract class AsapGithubException : Exception
{
    protected AsapGithubException() { }

    protected AsapGithubException(string? message) : base(message) { }

    protected AsapGithubException(string? message, Exception? innerException) : base(message, innerException) { }
}