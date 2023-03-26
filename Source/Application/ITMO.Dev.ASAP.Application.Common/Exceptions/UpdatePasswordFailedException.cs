namespace ITMO.Dev.ASAP.Application.Common.Exceptions;

public class UpdatePasswordFailedException : ApplicationException
{
    public UpdatePasswordFailedException(string? message) : base(message) { }
}