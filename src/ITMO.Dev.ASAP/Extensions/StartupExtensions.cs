using ITMO.Dev.ASAP.Github.Presentation.Webhooks.Extensions;
using ITMO.Dev.ASAP.Presentation.Rpc.Extensions;

namespace ITMO.Dev.ASAP.Extensions;

internal static class StartupExtensions
{
    internal static WebApplication Configure(this WebApplication app)
    {
        app.UseRequestLogging();

        if (app.Environment.IsDevelopment())
            app.UseWebAssemblyDebugging();

        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors(o => o.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

        app
            .UseBlazorFrameworkFiles()
            .UseStaticFiles()
            .UseRouting();

        if (app.Configuration.GetSection("Sentry:Enabled").Get<bool>())
        {
            app.UseSentryTracing();
        }

        app.MapRazorPages();

        app
            .UseAuthentication()
            .UseAuthorization();

        app.MapFallbackToFile("index.html");

        app.UseRouting();

        app.MapControllers();
        app.UseRpcPresentation();
        app.UseGithubIntegration();

        return app;
    }
}