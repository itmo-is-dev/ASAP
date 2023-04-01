namespace ITMO.Dev.ASAP.Identity.Exceptions;

public abstract class IdentityException : Exception
{
    protected IdentityException(string message)
        : base(message) { }
}