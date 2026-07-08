using Serilog;

namespace VariBillWebApp.Mvc.Extensions;

/// <summary>
/// Middleware pipeline configuration.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configures the MVC middleware pipeline.
    /// </summary>
    public static WebApplication ConfigureMvcPipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        return app;
    }
}