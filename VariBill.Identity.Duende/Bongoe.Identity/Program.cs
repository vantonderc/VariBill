using Bongoe.Identity.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure host (Serilog)
builder.ConfigureHost();

// Register services
builder.Services.AddIdentityServerServices(builder.Configuration);

// Build app
var app = builder.Build();

// Configure middleware pipeline
app.ConfigureIdentityServerPipeline();

app.Run();

