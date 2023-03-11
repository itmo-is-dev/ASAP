namespace ITMO.Dev.ASAP.Common.Exceptions;

public class RoleChangingException : DomainException
{
    public RoleChangingException(string? message)
        : base(message) { }
}