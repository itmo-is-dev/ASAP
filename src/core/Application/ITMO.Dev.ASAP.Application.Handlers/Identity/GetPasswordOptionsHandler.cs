using ITMO.Dev.ASAP.Application.Dto.Identity;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Queries.GetPasswordOptions;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;

internal class GetPasswordOptionsHandler : IRequestHandler<Query, Response>
{
    private readonly IOptionsSnapshot<IdentityOptions> _identityOptions;

    public GetPasswordOptionsHandler(IOptionsSnapshot<IdentityOptions> identityOptions)
    {
        _identityOptions = identityOptions;
    }

    public Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        IdentityOptions value = _identityOptions.Value;
        PasswordOptionsDto dto = value.ToDto();

        var response = new Response(dto);

        return Task.FromResult(response);
    }
}