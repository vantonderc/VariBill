using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Bongoe.Identity.Data;
using Bongoe.Identity.Models;


namespace Bongoe.Identity;

public static class HostingExtensions
{
    /// <summary>
    /// Extension methods to configure hosting for the Identity application (logging, services, pipeline).
    /// </summary>
    public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((ctx, lc) => lc
            .WriteTo.Console()
            .ReadFrom.Configuration(ctx.Configuration));
        return builder;
    }

    /// <summary>
    /// Configure services required for the Identity server and Identity UI.
    /// </summary>
    /// <returns>The built <see cref="WebApplication"/>.</returns>
    /// <remarks>
    /// Registers Identity, EF Core DbContext and IdentityServer with in-memory stores for development.
    /// </remarks>
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        // Add Razor Pages for the Identity UI (Login, Register, etc.)
        builder.Services.AddRazorPages();

        // Register the Identity DbContext with SQLite
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Add ASP.NET Core Identity with ApplicationUser and IdentityRole
        builder.Services
            .AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Configure password and lockout settings
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.SignIn.RequireConfirmedEmail = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI(); // Provides built‑in Login/Register pages

        // Configure the cookie settings
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Identity/Account/Login";
            options.LogoutPath = "/Identity/Account/Logout";
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
        });

        // Add IdentityServer with in‑memory configuration
        builder.Services
            .AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.EmitStaticAudienceClaim = true;
                options.KeyManagement.Enabled = false; // For development only
            })
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryApiResources(Config.ApiResources)
            .AddInMemoryClients(Config.Clients)
            .AddAspNetIdentity<ApplicationUser>()  // Bridges IdentityServer with ASP.NET Core Identity
            .AddDeveloperSigningCredential();      // Replace with a real certificate in production

        return builder.Build();
    }

    /// <summary>
    /// Configure the HTTP request pipeline for the Identity application.
    /// </summary>
    /// <param name="app">The application to configure.</param>
    /// <returns>The configured <see cref="WebApplication"/>.</returns>
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseRouting();

        // Authentication must be before IdentityServer
        app.UseAuthentication();
        app.UseIdentityServer();
        app.UseAuthorization();

        app.MapRazorPages();

        // Seed data only in development
        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();
            SeedData.EnsureSeedDataAsync(scope.ServiceProvider).GetAwaiter().GetResult();
        }

        return app;
    }
}
