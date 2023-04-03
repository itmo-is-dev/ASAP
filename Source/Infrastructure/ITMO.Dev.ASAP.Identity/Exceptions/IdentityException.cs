using Microsoft.AspNetCore.Identity;

namespace ITMO.Dev.ASAP.Identity.Exceptions;

public class IdentityException : Exception
{
    public IdentityException(IEnumerable<IdentityError> errors)
        : base(string.Join(' ', errors.Select(x => x.Description))) { }
}