namespace ITMO.Dev.ASAP.Application.Common.Exceptions;

public class IdentityOperationNotSuccededException : ApplicationException
{
    public IdentityOperationNotSuccededException(string? message)
        : base(message) { }
}