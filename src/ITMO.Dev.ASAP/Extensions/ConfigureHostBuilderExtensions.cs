using Serilog;

namespace ITMO.Dev.ASAP.Extensions;

public static class ConfigureHostBuilderExtensions
{
    public static IHostBuilder UseSerilogForAppLogs(this ConfigureHostBuilder hostBuilder, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.Sentry(options =>
            {
                configuration.GetSection("Sentry").Bind(options);
            })
            .CreateLogger();

        return hostBuilder.UseSerilog();
    }
}