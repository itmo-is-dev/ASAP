using ITMO.Dev.ASAP.Configuration;
using ITMO.Dev.ASAP.Github.Octokit.Extensions;
using ITMO.Dev.ASAP.Github.Presentation.Webhooks.Extensions;
using ITMO.Dev.ASAP.Playground.Github.Extensions;
using ITMO.Dev.ASAP.Playground.Github.TestEnv;
using Serilog;

namespace ITMO.Dev.ASAP.Playground.Github;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.File("AsapGithubPlayground.log")
            .CreateLogger();

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        TestEnvironmentConfiguration? testEnvironmentConfiguration = builder.Configuration
            .GetSection(nameof(TestEnvironmentConfiguration))
            .Get<TestEnvironmentConfiguration>();

        builder.Services
            .AddPlaygroundDependencies()
            .AddGithubServices(builder.Configuration);

        if (testEnvironmentConfiguration is not null)
            builder.Services.AddGithubPlaygroundDatabase(testEnvironmentConfiguration);

        WebApplication app = builder.Build();

        app.UseGithubIntegration();

        if (testEnvironmentConfiguration is not null)
            await app.Services.UseTestEnv(testEnvironmentConfiguration);

        await app.RunAsync();
    }
}