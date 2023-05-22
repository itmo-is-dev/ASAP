using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Github.Octokit.Configuration.ServiceClients;

public record ServiceClientCommand(IServiceCollection ServiceCollection, IConfiguration Configuration);