namespace ITMO.Dev.ASAP.Common.Exceptions;

public class AsapIdentityException : DomainException
{
    public AsapIdentityException(string? message)
        : base(message) { }
}