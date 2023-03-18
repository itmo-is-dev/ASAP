namespace ITMO.Dev.ASAP.Common.Exceptions;

public class UpdatePasswordFailedException : DomainException
{
    public UpdatePasswordFailedException(string? message) : base(message) { }
}