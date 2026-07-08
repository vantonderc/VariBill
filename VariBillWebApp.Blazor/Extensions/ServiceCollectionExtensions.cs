using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Polly;
using Polly.Extensions.Http;
using VariBillWebApp.Blazor.Services;
using VariBillWebApp.Blazor.Services.Abstractions;
using VariBillWebApp.Blazor.Settings;
using VariBillWebApp.Blazor.Utils;

namespace VariBillWebApp.Blazor.Extensions;

/// <summary>
/// Service registration extensions for Blazor Server.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configuration
        services.Configure<APISettings>(configuration.GetSection("APISettings"));

        // Blazor
        services.AddRazorPages();
        services.AddServerSideBlazor();

        // Antiforgery (for Blazor forms)
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
        .AddCookie()
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

        // HttpContextAccessor (for ApiClientHelper)
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
        services.AddScoped<IApiClient, ApiClientHelper>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductTypeService, ProductTypeService>();

        return services;
    }
}