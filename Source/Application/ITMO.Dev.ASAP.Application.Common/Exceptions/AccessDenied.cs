namespace ITMO.Dev.ASAP.Application.Common.Exceptions;

public class AccessDenied : ApplicationException
{
    public AccessDenied() : base("Access denied") { }
}