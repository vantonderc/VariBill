using VariBillWebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure host (Serilog)
builder.ConfigureHost();

// Register services
builder.Services.AddApiServices(builder.Configuration);

// Build app
var app = builder.Build();

// Configure middleware pipeline
app.ConfigureApiPipeline();

app.Run();



//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.EntityFrameworkCore;
//using Serilog;
//using VariBillWebAPI.Data.Context;
//using VariBillWebAPI.Data.Interceptors;
//using VariBillWebAPI.Data.Repository;
//using VariBillWebAPI.Data.Repository.Interface;
//using VariBillWebAPI.Data.Seeds;
//using VariBillWebAPI.Data.UnitOfWork;
//using VariBillWebAPI.Data.UnitOfWork.Interfaces;
//using VariBillWebAPI.Middleware;
//using VariBillWebAPI.Services;
//using VariBillWebAPI.Services.Caching;
//using VariBillWebAPI.Services.Interfaces;

//var builder = WebApplication.CreateBuilder(args);

//Log.Logger = new LoggerConfiguration()
//    .ReadFrom.Configuration(builder.Configuration)
//    .Enrich.FromLogContext()
//    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
//    .WriteTo.File("logs/variBill-.txt", rollingInterval: RollingInterval.Day)
//    .CreateLogger();

//builder.Host.UseSerilog();

//// Add services
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddOpenApi();

//// Database Context
//builder.Services.AddScoped<AuditInterceptor>();
//builder.Services.AddDbContext<VeriBillDBContext>((sp, options) =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
//    options.AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
//});

////builder.Services.AddDbContext<VeriBillTestDBContext>(opts =>
////    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//// CORS
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", policy =>
//    {
//        policy.AllowAnyOrigin()
//              .AllowAnyMethod()
//              .AllowAnyHeader();
//    });
//});

//// Authentication & Authorization
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.Authority = builder.Configuration["APISettings:IdentityServer"] ?? "http://localhost:5000";
//    options.RequireHttpsMetadata = false;
//    options.Audience = "VariBillWebAPI";
//    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
//    {
//        RoleClaimType = System.Security.Claims.ClaimTypes.Role
//    };
//});

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
//});

//builder.Services.AddMemoryCache();
//builder.Services.AddScoped<ICacheService, MemoryCacheService>();

//// Dependency Injection
//builder.Services.AddScoped<IProductService, ProductService>();
//builder.Services.AddScoped<IProductTypeService, ProductTypeService>();
//builder.Services.AddScoped<IProductRepository, ProductRepository>();
//builder.Services.AddScoped<IProductTypeRepository, ProductTypeRepository>();
//builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//builder.Services.AddScoped<DBContextSeedData>();
//builder.Services.AddHttpContextAccessor();

//var app = builder.Build();

//// Configure pipeline
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//    app.UseDeveloperExceptionPage();

//    // Seed data
//    using (var scope = app.Services.CreateScope())
//    {
//        var seed = scope.ServiceProvider.GetRequiredService<DBContextSeedData>();
//        await seed.SeedAllAsync();
//    }
//}
//else
//{
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseCors("AllowAll");
//app.UseAuthentication();
//app.UseAuthorization();
//app.MapControllers();

//app.UseMiddleware<GlobalExceptionMiddleware>();

//app.Run();



////// 1. Setup Serilog (Logging)

////using Microsoft.AspNetCore.Authentication.JwtBearer;
////using Microsoft.EntityFrameworkCore;
////using Serilog;
////using VariBillWebAPI.Controllers;
////using VariBillWebAPI.Data.Context;
////using VariBillWebAPI.Data.Repository;
////using VariBillWebAPI.Data.Repository.Interface;
////using VariBillWebAPI.Data.Seeds;
////using VariBillWebAPI.Data.UnitOfWork;
////using VariBillWebAPI.Data.UnitOfWork.Interfaces;
////using VariBillWebAPI.Services;
////using VariBillWebAPI.Services.Interfaces;
////using VariBillWebAPI.Settings;

//////TODO:maybe use logger from dt

////Log.Logger = new LoggerConfiguration()
////    //.WriteTo.File(
////    //    @"C:\var\logs\bongoe\bongoewebservice-.log",
////    //    rollingInterval: RollingInterval.Day,
////    //    fileSizeLimitBytes: 50_000_000,
////    //    retainedFileCountLimit: 10,
////    //    rollOnFileSizeLimit: true)
////    .CreateLogger();

////Log.Information("Starting Bongoe Web Service");

////try
////{
////    var builder = WebApplication.CreateBuilder(args);



////    // Sentry Configuration
////    //builder.WebHost.UseSentry(options => {
////    //    // Options can be configured here or in appsettings.json
////    //    options.TracesSampleRate = 1.0;
////    //});

////    builder.Services.AddLogging();

////    //// Health Checks
////    //builder.Services.AddHealthChecks()
////    //    .AddDbContextCheck<ModelContext>("Database");

////    //// OpenTelemetry Configuration
////    //builder.Services.AddOpenTelemetry()
////    //    .WithTracing(tracing =>
////    //    {
////    //        tracing.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("BongoeWebServiceDemo"))
////    //               .AddAspNetCoreInstrumentation()
////    //               .AddHttpClientInstrumentation()
////    //               .AddSqlClientInstrumentation()
////    //               .AddEntityFrameworkCoreInstrumentation()
////    //               .AddOtlpExporter()
////    //               .AddConsoleExporter();
////    //    })
////    //    .WithMetrics(metrics =>
////    //    {
////    //        metrics.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("BongoeWebServiceDemo"))
////    //               .AddAspNetCoreInstrumentation()
////    //               .AddHttpClientInstrumentation()
////    //               .AddRuntimeInstrumentation()
////    //               .AddOtlpExporter()
////    //               .AddConsoleExporter();
////    //    });


////    // 2. Add Services to the container (Old ConfigureServices)
////    var services = builder.Services;
////    var configuration = builder.Configuration;
////    var MyAllowSpecificOrigins = "BongoePolicy";

////    //  services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

////    // Configurations

////    services.Configure<APISettings>(configuration.GetSection("APISettings"));

////    services.AddDistributedMemoryCache();
////    services.AddOptions();

////    services.AddSession(options =>
////    {
////        options.IdleTimeout = TimeSpan.FromMinutes(5);
////        options.Cookie.HttpOnly = true;
////        options.Cookie.IsEssential = true;
////    });

////    // Required to access user tokens in the helper
////    services.AddHttpContextAccessor();

////    //services.AddTransient<BongoeM2MTokenHandler>();

////    // 2. Register the FileUploadApi with everything it needs
////    //builder.Services.AddHttpClient("FileUploadApi", (serviceProvider, client) =>
////    //services.AddHttpClient("FileUploadApi", (serviceProvider, client) =>
////    //{
////    //    var settings = serviceProvider.GetRequiredService<IOptions<APISettings>>().Value;

////    //    // Ensure we use the correct setting key
////    //    client.BaseAddress = new Uri(settings.FileUploadAPIAddress);
////    //    client.Timeout = TimeSpan.FromMinutes(5); // Important for large files
////    //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
////    //})
////    //.AddHttpMessageHandler<BongoeM2MTokenHandler>() // Automatically attaches the token
////    //.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
////    //{
////    //    // Fixes "The SSL connection could not be established" during local dev
////    //    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,
////    //    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
////    //})
////    //.AddPolicyHandler(GetRetryPolicy()) // From your Polly logic
////    //.AddPolicyHandler(GetCircuitBreakerPolicy()); // From your Polly logic




////    // Database Context
////    services.AddDbContext<VeriBillTestDBContext>(opts =>
////        opts.UseSqlServer(configuration.GetConnectionString("DefaultConnection")),
////        ServiceLifetime.Scoped);


////    //// Controllers & SignalR
////    services.AddControllers().AddJsonOptions(options =>
////    {
////        //options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;// Newtonsoft.Json.ReferenceLoopHandling.Ignore;
////    });



////    //services.AddSignalR();

////    // CORS
////    services.AddCors(options =>
////    {
////        options.AddPolicy(name: MyAllowSpecificOrigins, builder =>
////        {
////            builder.AllowAnyOrigin()
////                   .AllowAnyMethod()
////                   .AllowAnyHeader();
////        });
////    });

////    // Authentication & Authorization
////    services.AddAuthentication(options =>
////    {
////        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
////        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
////    })
////    .AddJwtBearer("Bearer", options =>
////    {
////        options.Authority = "http://localhost:5000";
////        options.RequireHttpsMetadata = false;
////        options.Audience = "VariBillWebAPI";
////        /*options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
////        {
////            ValidateAudience = false // This stops the 401 if the 'aud' claim doesn't match exactly
////        };*/
////    });

////    services.AddAuthorization();

////    // Application Services (DI)
////    //TODO:remove controllers from DI if not needed, as they can be resolved by the framework automatically. Only add to DI if you need to inject them somewhere else.
////    //services.AddTransient<ProductController>();
////    // Injected into other controllers for bid/cert bootstrap flows.

////    services.AddTransient<DBContextSeedData>();


////    services.AddScoped<IProductService, ProductService>();
////    services.AddScoped<IProductTypeService, ProductTypeService>();

////    services.AddScoped<IProductRepository, ProductRepository>();
////    services.AddScoped<IProductTypeRepository, ProductTypeRepository>();



////    // HTTP Context & Accessor
////    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

////    // Register Unit of Work
////    services.AddScoped<IUnitOfWork, UnitOfWork>();



////    // Logger DI Fix****
////    //services.AddSingleton<Microsoft.Extensions.Logging.ILogger>(svc =>
////    //    svc.GetRequiredService<ILogger<ProductController>>());

////    // Register the standard IHttpClientFactory
////    //services.AddHttpClient();

////    //services.Configure<IISServerOptions>(options =>
////    //{
////    //    options.AutomaticAuthentication = false;
////    //});

////    //// Compression
////    //services.AddResponseCompression(options =>
////    //{
////    //    options.EnableForHttps = true;
////    //    options.MimeTypes = new[] { "text/plain" };
////    //});

////    //services.Configure<GzipCompressionProviderOptions>(options =>
////    //{
////    //    options.Level = CompressionLevel.Fastest;
////    //});

////    services.AddOpenApi();

////    var app = builder.Build();

////    // 3. Configure the HTTP request pipeline (Old Configure)
////    if (app.Environment.IsDevelopment())
////    {
////        app.MapOpenApi();
////        app.UseDeveloperExceptionPage();
////    }
////    else
////    {
////        app.UseHsts();
////    }

////    if (!app.Environment.IsDevelopment())
////    {
////        app.UseHttpsRedirection();
////    }
////    app.UseRouting();
////    app.UseSession();
////    //app.UseResponseCompression();

////    app.UseCors(MyAllowSpecificOrigins);

////    app.UseAuthentication();
////    app.UseAuthorization();

////    // 4. Seeding Data - Run in Development environment
////    if (app.Environment.IsDevelopment())
////    {
////        using (var scope = app.Services.CreateScope())
////        {
////            var seed = scope.ServiceProvider.GetRequiredService<DBContextSeedData>();
////            await seed.SeedAllAsync();
////        }
////    }

////    // 5. Endpoints
////    app.MapControllers();
////    app.Run();
////}
////catch (Exception ex)
////{
////    // This is the "Gold Mine" of info for DI errors
////    Console.WriteLine("--------------------------------------------------");
////    Console.WriteLine("CRITICAL DI ERROR: " + ex.Message);
////    if (ex.InnerException != null)
////    {
////        Console.WriteLine("INNER EXCEPTION: " + ex.InnerException.Message);
////    }
////    Console.WriteLine("--------------------------------------------------");
////    Console.WriteLine("Press any key to see the full stack trace...");
////    Console.ReadKey();
////    throw;
////}

//////finally
//////{
//////    Log.CloseAndFlush();
//////}


//////static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
//////{
//////    var jitterer = new Random();
//////    return HttpPolicyExtensions
//////        .HandleTransientHttpError()
//////        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
//////        .WaitAndRetryAsync(3, retryAttempt =>
//////            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
//////            + TimeSpan.FromMilliseconds(jitterer.Next(0, 100))); // Adds up to 100ms of "fuzziness"
//////}


//////static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
//////{
//////    return HttpPolicyExtensions
//////        .HandleTransientHttpError()
//////        .CircuitBreakerAsync(
//////            handledEventsAllowedBeforeBreaking: 5,
//////            durationOfBreak: TimeSpan.FromSeconds(30),
//////            onBreak: (outcome, timespan) =>
//////            {
//////                Log.Warning("CIRCUIT BREAKER: Tripped for {0}s due to: {1}",
//////                    timespan.TotalSeconds, outcome.Exception?.Message);
//////            },
//////            onReset: () => Log.Information("CIRCUIT BREAKER: Reset and allowing traffic again.")
//////        );
//////}















//////// 1.Setup Serilog(Logging)
////////Log.Logger = new LoggerConfiguration()
////////    .WriteTo.RollingFileAlternate(@"C:\var\logs\bongoe\bongoewebservice", fileSizeLimitBytes: 50000000, retainedFileCountLimit: 10)
////////    .CreateLogger();

////////Log.Logger = new LoggerConfiguration()
////////    .WriteTo.File(
////////        @"C:\var\logs\bongoe\bongoewebservice-.log",
////////        rollingInterval: RollingInterval.Day,
////////        fileSizeLimitBytes: 50_000_000,
////////        retainedFileCountLimit: 10,
////////        rollOnFileSizeLimit: true)
////////    .CreateLogger();

////////Log.Information("Starting Bongoe Web Service");

////////try
////////{


//////using Microsoft.AspNetCore.Authentication.JwtBearer;
//////using Microsoft.AspNetCore.ResponseCompression;
//////using Microsoft.Extensions.Options;
//////using System.IO.Compression;
//////using VariBillWebAPI.Controllers;
//////using VariBillWebAPI.Services.Interfaces;
//////using VariBillWebAPI.Settings;

//////var builder = WebApplication.CreateBuilder(args);

////////// Sentry Configuration
////////builder.WebHost.UseSentry(options => {
////////    // Options can be configured here or in appsettings.json
////////    options.TracesSampleRate = 1.0;
////////});

////////builder.Services.AddLogging();

////////// Health Checks
////////builder.Services.AddHealthChecks()
////////    .AddDbContextCheck<ModelContext>("Database");

////////// OpenTelemetry Configuration
////////builder.Services.AddOpenTelemetry()
////////    .WithTracing(tracing =>
////////    {
////////        tracing.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("BongoeWebServiceDemo"))
////////               .AddAspNetCoreInstrumentation()
////////               .AddHttpClientInstrumentation()
////////               .AddSqlClientInstrumentation()
////////               .AddEntityFrameworkCoreInstrumentation()
////////               .AddOtlpExporter()
////////               .AddConsoleExporter();
////////    })
////////    .WithMetrics(metrics =>
////////    {
////////        metrics.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("BongoeWebServiceDemo"))
////////               .AddAspNetCoreInstrumentation()
////////               .AddHttpClientInstrumentation()
////////               .AddRuntimeInstrumentation()
////////               .AddOtlpExporter()
////////               .AddConsoleExporter();
////////    });


////////// 2. Add Services to the container (Old ConfigureServices)
//////var services = builder.Services;
//////var configuration = builder.Configuration;
//////var MyAllowSpecificOrigins = "BongoePolicy";//TODO:ensure this is used, rename

////////services.AddDistributedMemoryCache();
//////services.AddOptions();

////////services.AddSession(options =>
////////{
////////    options.IdleTimeout = TimeSpan.FromMinutes(5);
////////    options.Cookie.HttpOnly = true;
////////    options.Cookie.IsEssential = true;
////////});

////////// Required to access user tokens in the helper
////////services.AddHttpContextAccessor();


//////////2.Register the FileUploadApi with everything it needs
////////builder.Services.AddHttpClient("FileUploadApi", (serviceProvider, client) =>
////////services.AddHttpClient("FileUploadApi", (serviceProvider, client) =>
////////{
////////    var settings = serviceProvider.GetRequiredService<IOptions<APISettings>>().Value;

////////// Ensure we use the correct setting key
////////client.BaseAddress = new Uri(settings.FileUploadAPIAddress);
////////client.Timeout = TimeSpan.FromMinutes(5); // Important for large files
////////client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
////////})
////////.AddHttpMessageHandler<BongoeM2MTokenHandler>() // Automatically attaches the token
////////.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
////////{
////////    // Fixes "The SSL connection could not be established" during local dev
////////    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,
////////    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
////////})
////////.AddPolicyHandler(GetRetryPolicy()) // From your Polly logic
////////.AddPolicyHandler(GetCircuitBreakerPolicy()); // From your Polly logic


//////services.AddCors(options =>
//////{
//////    options.AddPolicy(name: MyAllowSpecificOrigins, builder =>
//////    {
//////        builder.AllowAnyOrigin()
//////               .AllowAnyMethod()
//////               .AllowAnyHeader();
//////    });
//////});

//////// Add services to the container.

//////builder.Services.AddControllers();

////////TODO:maybe use server discovery oro endpoint discovery or soemthiign...
////////********************************************************

//////// Authentication & Authorization
//////services.AddAuthentication(options =>
//////{
//////    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//////    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//////})
//////.AddJwtBearer("Bearer", options =>
//////{
//////    options.Authority = "http://localhost:5000";
//////    options.RequireHttpsMetadata = false;
//////    options.Audience = "VariBillWebAPI";
//////    /*options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
//////    {
//////        ValidateAudience = false // This stops the 401 if the 'aud' claim doesn't match exactly
//////    };*/
//////});

//////services.AddAuthorization();

////////TODO:NB->move all these to sepearate configus extensions etxc.....
//////// Application Services (DI)
////////TODO:remove controllers from DI if not needed, as they can be resolved by the framework automatically. Only add to DI if you need to inject them somewhere else.
////////services.AddTransient<ProductController>();
//////// Injected into other controllers for bid/cert bootstrap flows.
////////services.AddTransient<DBContextSeedData>();

////////services.AddScoped<IProductService, ProductService>();





//////// Register Repositories
//////// Core Entity Repositories
////////services.AddScoped<IProductRepository, ProductRepository>();

////////// Register Unit of Work
////////services.AddScoped<IUnitOfWork, UnitOfWork>();


////////// Register the standard IHttpClientFactory
////////services.AddHttpClient();



////////services.Configure<IISServerOptions>(options =>
////////{
////////    options.AutomaticAuthentication = false;
////////});

//////// Compression
////////services.AddResponseCompression(options =>
////////{
////////    options.EnableForHttps = true;
////////    options.MimeTypes = new[] { "text/plain" };
////////});

////////services.Configure<GzipCompressionProviderOptions>(options =>
////////{
////////    options.Level = CompressionLevel.Fastest;
////////});

////////services.AddOpenApi();
////////******************************************************

//////var app = builder.Build();

//////    // Configure the HTTP request pipeline.

//////    app.UseHttpsRedirection();

//////    app.UseAuthorization();

//////    app.MapControllers();


////////*****************************************************

////////3.Configure the HTTP request pipeline (Old Configure)
//////if (app.Environment.IsDevelopment())
//////{
//////    app.MapOpenApi();
//////app.UseDeveloperExceptionPage();
//////}
//////else
//////{
//////    app.UseHsts();
//////}

//////if (!app.Environment.IsDevelopment())
//////{
//////    app.UseHttpsRedirection();
//////}
//////app.UseRouting();
//////app.UseSession();
//////app.UseResponseCompression();

//////app.UseCors(MyAllowSpecificOrigins);

//////app.UseAuthentication();
//////app.UseAuthorization();

//////// 4. Seeding Data

////////var shouldSeed = builder.Configuration.GetValue<bool>("SeedSettings:Enabled");
////////if (shouldSeed && app.Environment.IsDevelopment())
////////{
////////    using (var scope = app.Services.CreateScope())
////////    {
////////        var seed = scope.ServiceProvider.GetRequiredService<DBContextSeedData>();
////////        await seed.SeedAllAsync();
////////    }
////////}


////////// 5. Endpoints
////////app.MapHealthChecks("/health");
//////app.MapControllers();
////////app.MapHub<MessageHub>("/hubs/messagehub");

////////app.MapPost("/admin/ml/train-trust-model", async (IProfileTrustPredictionService mlService) =>
////////{
////////    var result = await mlService.TrainAndSaveModelAsync();
////////    return Results.Ok(result);
////////})
////////.RequireAuthorization("AdminOnly");



//////app.Run();
////////}
////////catch (Exception ex)
////////{
////////    // This is the "Gold Mine" of info for DI errors
////////    Console.WriteLine("--------------------------------------------------");
////////    Console.WriteLine("CRITICAL DI ERROR: " + ex.Message);
////////    if (ex.InnerException != null)
////////    {
////////        Console.WriteLine("INNER EXCEPTION: " + ex.InnerException.Message);
////////    }
////////    Console.WriteLine("--------------------------------------------------");
////////    Console.WriteLine("Press any key to see the full stack trace...");
////////    Console.ReadKey();
////////    throw;
////////}

////////finally
////////{
////////    Log.CloseAndFlush();
////////}
