//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.EntityFrameworkCore;
//using Serilog;
//using VariBillWebAPI.Data.Context;
//using VariBillWebAPI.Data.Interceptors;
//using VariBillWebAPI.Data.Repository;
//using VariBillWebAPI.Data.Repository.Interfaces;
//using VariBillWebAPI.Data.Seeds;
//using VariBillWebAPI.Data.UnitOfWork;
//using VariBillWebAPI.Data.UnitOfWork.Interfaces;
//using VariBillWebAPI.Middleware;
//using VariBillWebAPI.Services;
//using VariBillWebAPI.Services.Abstractions;
//using VariBillWebAPI.Services.Caching;
//using VariBillWebAPI.Settings;

//var builder = WebApplication.CreateBuilder(args);

//// ============================================================
//// 1. Serilog Configuration
//// ============================================================
//Log.Logger = new LoggerConfiguration()
//    .ReadFrom.Configuration(builder.Configuration)
//    .Enrich.FromLogContext()
//    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
//    .WriteTo.File("logs/api-.txt", rollingInterval: RollingInterval.Day)
//    .CreateLogger();

//builder.Host.UseSerilog();

//// ============================================================
//// 2. Configuration
//// ============================================================
//builder.Services.Configure<APISettings>(builder.Configuration.GetSection("APISettings"));

//// ============================================================
//// 3. Controllers
//// ============================================================
//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
//        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
//    });

//// ============================================================
//// 4. Swagger / OpenAPI
//// ============================================================
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
//    {
//        Title = "VariBill API",
//        Version = "v1"
//    });
//    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
//    {
//        Description = "JWT Authorization header using the Bearer scheme.",
//        Name = "Authorization",
//        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
//        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
//        Scheme = "Bearer"
//    });
//    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
//    {
//        {
//            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
//            {
//                Reference = new Microsoft.OpenApi.Models.OpenApiReference
//                {
//                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            Array.Empty<string>()
//        }
//    });
//});

//// ============================================================
//// 5. Caching
//// ============================================================
//builder.Services.AddDistributedMemoryCache(); // For session
//builder.Services.AddMemoryCache();           // For ICacheService
//builder.Services.AddScoped<ICacheService, MemoryCacheService>();

//// ============================================================
//// 6. Session
//// ============================================================
//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromMinutes(5);
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;
//});

//// ============================================================
//// 7. HttpContextAccessor
//// ============================================================
//builder.Services.AddHttpContextAccessor();

//// ============================================================
//// 8. Database Context
//// ============================================================
//builder.Services.AddScoped<AuditInterceptor>();

//builder.Services.AddDbContext<VeriBillTestDBContext>((sp, options) =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
//    {
//        sqlOptions.EnableRetryOnFailure(
//            maxRetryCount: 5,
//            maxRetryDelay: TimeSpan.FromSeconds(30),
//            errorNumbersToAdd: null);
//    });
//    options.UseLazyLoadingProxies();
//    options.AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
//});

//// ============================================================
//// 9. CORS
//// ============================================================
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", policy =>
//    {
//        policy.AllowAnyOrigin()
//              .AllowAnyMethod()
//              .AllowAnyHeader();
//    });
//});

//// ============================================================
//// 10. Authentication & Authorization
//// ============================================================
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.Authority = builder.Configuration["APISettings:IdentityServer"] ?? "http://localhost:5000";
//        options.RequireHttpsMetadata = false;
//        options.Audience = "VariBillWebAPI";
//        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
//        {
//            RoleClaimType = System.Security.Claims.ClaimTypes.Role,
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true
//        };
//    });

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
//});

//// ============================================================
//// 11. Health Checks
//// ============================================================
//builder.Services.AddHealthChecks()
//    .AddDbContextCheck<VeriBillTestDBContext>();

//// ============================================================
//// 12. Response Caching
//// ============================================================
//builder.Services.AddResponseCaching();

//// ============================================================
//// 13. Dependency Injection
//// ============================================================
//builder.Services.AddScoped<IProductService, ProductService>();
//builder.Services.AddScoped<IProductTypeService, ProductTypeService>();
//builder.Services.AddScoped<IProductRepository, ProductRepository>();
//builder.Services.AddScoped<IProductTypeRepository, ProductTypeRepository>();
//builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//builder.Services.AddScoped<DBContextSeedData>();

//// ============================================================
//// 14. Build App
//// ============================================================
//var app = builder.Build();

//// ============================================================
//// 15. Middleware Pipeline
//// ============================================================

//// Global Exception Handler
//app.UseMiddleware<GlobalExceptionMiddleware>();

//// Serilog Request Logging
//app.UseSerilogRequestLogging();

//// Development/Production
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//    app.UseDeveloperExceptionPage();
//}
//else
//{
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseResponseCaching();
//app.UseCors("AllowAll");
//app.UseRouting();
//app.UseSession();
//app.UseAuthentication();
//app.UseAuthorization();

//// Health Checks
//app.MapHealthChecks("/health");

//// Controllers
//app.MapControllers();

//// ============================================================
//// 16. Seed Data (Development Only)
//// ============================================================
//if (app.Environment.IsDevelopment())
//{
//    using var scope = app.Services.CreateScope();
//    var seed = scope.ServiceProvider.GetRequiredService<DBContextSeedData>();
//    await seed.SeedAllAsync();
//}

//// ============================================================
//// 17. Run
//// ============================================================
//app.Run();
