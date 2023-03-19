using ITMO.Dev.ASAP.Mapping.Mappings;
using Microsoft.Extensions.Configuration;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Queries.GetPasswordOptions;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;
internal class GetPasswordOptionsHandler
{
    private readonly IConfigurationRoot _identityConfigurationRoot = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    public Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        IConfigurationSection configurationSection = _identityConfigurationRoot
            .GetSection("Identity")
            .GetSection("IdentityConfiguration");
        return Task.FromResult(new Response(configurationSection.ToPasswordOptionsDto()));
    }
}