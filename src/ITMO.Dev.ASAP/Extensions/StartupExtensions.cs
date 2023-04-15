using ITMO.Dev.ASAP.Github.Presentation.Webhooks.Extensions;

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

        app.MapRazorPages();

        app
            .UseAuthentication()
            .UseAuthorization();

        app.MapFallbackToFile("index.html");

        app.MapControllers();
        app.UseGithubIntegration();

        return app;
    }
}