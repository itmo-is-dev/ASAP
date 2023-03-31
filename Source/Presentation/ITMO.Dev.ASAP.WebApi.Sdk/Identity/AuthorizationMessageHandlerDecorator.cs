using System.Net.Http.Headers;

namespace ITMO.Dev.ASAP.WebApi.Sdk.Identity;

public class AuthorizationMessageHandlerDecorator : DelegatingHandler
{
    private readonly ITokenProvider _identityProvider;

    public AuthorizationMessageHandlerDecorator(ITokenProvider identityProvider)
    {
        _identityProvider = identityProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        string? identity = await _identityProvider.FindIdentityAsync(cancellationToken);

        if (identity is not null)
            request.Headers.Authorization = AuthenticationHeaderValue.Parse($"Bearer {identity}");

        return await base.SendAsync(request, cancellationToken);
    }
}