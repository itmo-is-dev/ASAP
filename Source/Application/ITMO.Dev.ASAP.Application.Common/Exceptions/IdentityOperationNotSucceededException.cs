namespace ITMO.Dev.ASAP.Application.Common.Exceptions;

public class IdentityOperationNotSucceededException : ApplicationException
{
    public IdentityOperationNotSucceededException(string? message)
        : base(message) { }
}