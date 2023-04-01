namespace ITMO.Dev.ASAP.Identity.Exceptions;

public class UpdatePasswordFailedException : IdentityException
{
    public UpdatePasswordFailedException(string message)
        : base(message) { }
}
