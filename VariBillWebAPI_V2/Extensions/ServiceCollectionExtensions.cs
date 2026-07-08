using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using VariBillWebAPI.Data.Context;
using VariBillWebAPI.Data.Interceptors;
using VariBillWebAPI.Data.Repository;
using VariBillWebAPI.Data.Repository.Interfaces;
using VariBillWebAPI.Data.Seeds;
using VariBillWebAPI.Data.UnitOfWork;
using VariBillWebAPI.Data.UnitOfWork.Interfaces;
using VariBillWebAPI.HealthChecks;
using VariBillWebAPI.Services;
using VariBillWebAPI.Services.Abstractions;
using VariBillWebAPI.Services.Caching;
using VariBillWebAPI.Services.Interfaces;
using VariBillWebAPI.Settings;

namespace VariBillWebAPI.Extensions;

/// <summary>
/// Service registration extensions.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Extension helpers for registering API services and dependencies.
    /// </summary>
    /// <summary>
    /// Registers all API services.
    /// </summary>
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        /// <summary>
        /// Registers services used by the VariBill API such as DbContext, repositories, services and health checks.
        /// </summary>
        // Load settings
        services.Configure<APISettings>(configuration.GetSection("APISettings"));

        // Controllers + JSON options
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });

        // Swagger / OpenAPI
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "VariBill API", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });

        // Database
        services.AddDbContext<VeriBillTestDBContext>((sp, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
            });
            options.UseLazyLoadingProxies(); // Enable lazy loading
            options.AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
        });

        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        // Authentication & Authorization
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["APISettings:IdentityServer"] ?? "http://localhost:5000";
                options.RequireHttpsMetadata = false;
                options.Audience = "VariBillWebAPI";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    RoleClaimType = System.Security.Claims.ClaimTypes.Role,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
        });

        // Caching
        services.AddMemoryCache();
        services.AddScoped<ICacheService, MemoryCacheService>();

        // Health checks
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database")
            .AddCheck<CacheHealthCheck>("cache");

        // Response caching
        services.AddResponseCaching();

        // Dependency Injection
        services.AddScoped<AuditInterceptor>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductTypeService, ProductTypeService>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductTypeRepository, ProductTypeRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<DBContextSeedData>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // HttpContextAccessor
        services.AddHttpContextAccessor();

        return services;
    }
}
