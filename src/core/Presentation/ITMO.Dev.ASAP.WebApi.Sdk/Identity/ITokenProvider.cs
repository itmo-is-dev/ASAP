namespace ITMO.Dev.ASAP.WebApi.Sdk.Identity;

public interface ITokenProvider
{
    ValueTask<string?> FindIdentityAsync(CancellationToken cancellationToken = default);
}