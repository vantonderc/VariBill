using Serilog;

namespace VariBillWebApp.Blazor.Extensions;

/// <summary>
/// Middleware pipeline configuration.
/// </summary>
public static class ApplicationBuilderExtensions
{
    public static WebApplication ConfigureBlazorPipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        return app;
    }
}