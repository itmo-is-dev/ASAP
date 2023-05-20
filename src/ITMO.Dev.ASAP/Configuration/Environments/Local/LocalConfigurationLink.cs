using FluentChaining;

namespace ITMO.Dev.ASAP.Configuration.Environments.Local;

public class LocalConfigurationLink : IAsyncLink<ConfigurationCommand>
{
    private const string EnvironmentName = "local";

    public Task<Unit> Process(
        ConfigurationCommand request,
        AsynchronousContext context,
        LinkDelegate<ConfigurationCommand, AsynchronousContext, Task<Unit>> next)
    {
        return request.Environment.Equals(EnvironmentName, StringComparison.OrdinalIgnoreCase)
            ? Unit.Task
            : next(request, context);
    }
}