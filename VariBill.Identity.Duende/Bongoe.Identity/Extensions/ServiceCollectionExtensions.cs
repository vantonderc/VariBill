using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Bongoe.Identity.Data;
using Bongoe.Identity.Models;

namespace Bongoe.Identity.Extensions;

/// <summary>
/// Service registration extensions.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers IdentityServer, Identity, and database services.
    /// </summary>
    public static IServiceCollection AddIdentityServerServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Razor Pages for Identity UI (login, register, etc.)
        services.AddRazorPages();

        // DbContext for Identity (SQLite)
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        // ASP.NET Core Identity with ApplicationUser
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.SignIn.RequireConfirmedEmail = false;
            options.User.RequireUniqueEmail = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders()
        .AddDefaultUI(); // Built‑in Login/Register pages

        // Cookie configuration
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Identity/Account/Login";
            options.LogoutPath = "/Identity/Account/Logout";
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });

        // Duende IdentityServer
        services.AddIdentityServer(options =>
        {
            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;
            options.EmitStaticAudienceClaim = true;
            options.KeyManagement.Enabled = false; // Development only – use proper certificate in production
        })
        .AddInMemoryIdentityResources(Config.IdentityResources)
        .AddInMemoryApiScopes(Config.ApiScopes)
        .AddInMemoryApiResources(Config.ApiResources)
        .AddInMemoryClients(Config.Clients)
        .AddAspNetIdentity<ApplicationUser>() // Bridges IdentityServer with ASP.NET Core Identity
        .AddDeveloperSigningCredential(); // Replace with real certificate in production

        return services;
    }
}