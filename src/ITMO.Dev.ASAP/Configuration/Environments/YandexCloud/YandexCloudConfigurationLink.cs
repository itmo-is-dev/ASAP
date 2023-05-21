using FluentChaining;
using ITMO.Dev.ASAP.Configuration;
using ITMO.Dev.ASAP.Configuration.Environments;
using ITMO.Dev.ASAP.Exceptions;
using ITMO.Dev.ASAP.Extensions;

namespace ITMO.Dev.ASAP;

internal class YandexCloudConfigurationLink : IAsyncLink<ConfigurationCommand>
{
    private const string EnvironmentName = "YandexCloud";

    public async Task<Unit> Process(
        ConfigurationCommand request,
        AsynchronousContext context,
        LinkDelegate<ConfigurationCommand, AsynchronousContext, Task<Unit>> next)
    {
        if (request.Environment.Equals(EnvironmentName, StringComparison.OrdinalIgnoreCase) is false)
        {
            return await next(request, context);
        }

        string secretId = request.ApplicationBuilder.Configuration.GetValue<string>("Deployment:SecretId")
                          ?? throw new StartupException("SecretId must be defined for Yandex Cloud deployment");

        string token = await YandexTokenProvider.GetToken();
        var lockBoxClient = new YandexLockBoxClient(token);
        LockBoxEntry[] entries = await lockBoxClient.GetEntries(secretId);
        request.ApplicationBuilder.Configuration.AddSecretsFromLockBox(entries);

        return Unit.Value;
    }
}