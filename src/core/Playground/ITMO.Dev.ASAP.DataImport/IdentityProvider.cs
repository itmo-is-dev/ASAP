using ITMO.Dev.ASAP.WebApi.Sdk.Identity;

namespace ITMO.Dev.ASAP.DataImport;

public class IdentityProvider : ITokenProvider
{
    public string? UserIdentity { get; set; }

    public ValueTask<string?> FindIdentityAsync(CancellationToken cancellationToken = default)
        => ValueTask.FromResult(UserIdentity);
}