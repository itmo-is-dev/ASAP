namespace ITMO.Dev.ASAP.Common.Exceptions;

public class UpdateUsernameFailedException : DomainException
{
    public UpdateUsernameFailedException(string? message) : base(message) { }
}