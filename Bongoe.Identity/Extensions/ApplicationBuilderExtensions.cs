using Microsoft.AspNetCore.Builder;
using Bongoe.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Bongoe.Identity.Extensions;

/// <summary>
/// Middleware pipeline configuration.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Extension methods to configure the IdentityServer request pipeline.
    /// </summary>
    /// <seealso cref="Bongoe.Identity.Data.ApplicationDbContext" />
    public static WebApplication ConfigureIdentityServerPipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseRouting();


        app.UseCors("BongoePolicy");

        app.UseAuthentication();

       

        app.UseIdentityServer();
        app.UseAuthorization();

        app.MapRazorPages();

        // Seed data (development only)
        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();
            SeedData.EnsureSeedDataAsync(scope.ServiceProvider).GetAwaiter().GetResult();
        }

        // Apply migrations (optional)
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.Migrate();
        }

        return app;
    }
}
