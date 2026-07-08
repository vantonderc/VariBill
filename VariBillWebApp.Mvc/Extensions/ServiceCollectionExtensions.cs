using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Polly;
using Polly.Extensions.Http;
using VariBillWebApp.Mvc.Helpers;
using VariBillWebApp.Mvc.Services;
using VariBillWebApp.Mvc.Services.Abstractions;
using VariBillWebApp.Mvc.Settings;
using VariBillWebApp.Mvc.Utils;

namespace VariBillWebApp.Mvc.Extensions;

/// <summary>
/// Service registration extensions for the MVC application.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all MVC services, authentication, and HTTP clients.
    /// </summary>
    public static IServiceCollection AddMvcServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configuration
        services.Configure<APISettings>(configuration.GetSection("APISettings"));
        services.AddOptions();

        // MVC
        services.AddControllersWithViews();

        // Antiforgery (AJAX header support)
        services.AddAntiforgery(options =>
        {
            options.HeaderName = "RequestVerificationToken";
        });

        // Authentication: Cookie + OpenID Connect
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie(options =>
        {
            // For AJAX calls, return 401/403 status instead of redirect
            options.Events.OnRedirectToLogin = ctx =>
            {
                if (IsAjaxRequest(ctx.Request))
                {
                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                }
                /*
                    if (context.Request.Path.StartsWithSegments("/catalog") 
                        || context.Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                        || context.Request.Headers.Accept.ToString().Contains("application/json"))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    }
                */

                ctx.Response.Redirect(ctx.RedirectUri);
                return Task.CompletedTask;
            };
            options.Events.OnRedirectToAccessDenied = ctx =>
            {
                if (IsAjaxRequest(ctx.Request))
                {
                    ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                }
                ctx.Response.Redirect(ctx.RedirectUri);
                return Task.CompletedTask;
            };
        })
        .AddOpenIdConnect(options =>
        {
            var apiSettings = configuration.GetSection("APISettings").Get<APISettings>()!;

            options.Authority = apiSettings.IdentityServer;
            options.ClientId = apiSettings.InteractiveClientId;
            options.ClientSecret = apiSettings.APISecret;
            options.ResponseType = "code";
            options.UsePkce = true;
            options.SaveTokens = true;
            options.GetClaimsFromUserInfoEndpoint = true;

            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("email");
            options.Scope.Add("roles");
            options.Scope.Add(apiSettings.APIScope);

            options.TokenValidationParameters = new()
            {
                NameClaimType = "name",
                RoleClaimType = "role"
            };

            options.RequireHttpsMetadata = false; // Set to true in production
        });

        // Authorization policies
        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
        });

        // HTTP context accessor
        services.AddHttpContextAccessor();

        // HTTP client with Polly retry
        services.AddHttpClient("VariBillApi", client =>
        {
            var settings = configuration.GetSection("APISettings").Get<APISettings>();
            client.BaseAddress = new Uri(settings!.VariBillAPIAddress);
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        })
        .AddPolicyHandler(HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

        // Application services
        services.AddScoped<APIValidation>();
        services.AddScoped<ApiClientHelper>();
        services.AddScoped<IVariBillApiClient>(sp => sp.GetRequiredService<ApiClientHelper>());
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped< IProductTypeService, ProductTypeService>();

        return services;
    }

    private static bool IsAjaxRequest(HttpRequest request) =>
        request.Headers.XRequestedWith == "XMLHttpRequest" ||
        request.Headers.Accept.Any(h => h is not null && h.Contains("application/json", StringComparison.OrdinalIgnoreCase));
}




