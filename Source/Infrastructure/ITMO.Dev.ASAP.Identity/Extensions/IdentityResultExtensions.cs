using ITMO.Dev.ASAP.Application.Common.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace ITMO.Dev.ASAP.Identity.Extensions;

internal static class IdentityResultExtensions
{
    public static void EnsureSucceded(this IdentityResult result)
    {
        if (result.Succeeded is false)
            throw new IdentityOperationNotSuccededException(string.Join(' ', result.Errors.Select(x => x.Description)));
    }
}
