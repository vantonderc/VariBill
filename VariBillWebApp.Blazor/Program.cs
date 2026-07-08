using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Net.Http.Headers;
using VariBillWebApp.Blazor.Services;
using VariBillWebApp.Blazor.Services.Abstractions;
using VariBillWebApp.Blazor.Settings;
using VariBillWebApp.Blazor.Utils;


var builder = WebApplication.CreateBuilder(args);

// Load API settings
builder.Services.Configure<APISettings>(builder.Configuration.GetSection("APISettings"));

// Add Blazor Server services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Antiforgery for Blazor forms
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});

// ---------- Authentication ----------
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect(options =>
{
    var apiSettings = builder.Configuration.GetSection("APISettings").Get<APISettings>()!;

    options.Authority = apiSettings.IdentityServer;
    options.ClientId = apiSettings.InteractiveClientId;
    options.ClientSecret = apiSettings.APISecret;
    options.ResponseType = "code";
    options.UsePkce = true;
    options.SaveTokens = true; // Store the user's access token for forwarding
    options.GetClaimsFromUserInfoEndpoint = true;

    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("roles");
    options.Scope.Add("VariBillWebAPI");

    options.TokenValidationParameters = new()
    {
        NameClaimType = "name",
        RoleClaimType = "role"
    };

    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
});

builder.Services.AddHttpContextAccessor();

// Configure HttpClient for API calls
builder.Services.AddHttpClient("VariBillApi", client =>
{
    var settings = builder.Configuration.GetSection("APISettings").Get<APISettings>();
    client.BaseAddress = new Uri(settings!.VariBillAPIAddress);
    client.Timeout = TimeSpan.FromSeconds(builder.Environment.IsDevelopment() ? 100 : 30);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

// Register services
builder.Services.AddScoped<IApiClient, ApiClientHelper>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductTypeService, ProductTypeService>();
builder.Services.AddScoped<APIValidation>();

var app = builder.Build();

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

app.Run();
