using VariBillWebApp.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Quick runtime check of the configured API address (safe before Build)
var apiAddr = builder.Configuration["APISettings:VariBillAPIAddress"];

// Configure host (Serilog)
builder.ConfigureHost();    

// Register services
builder.Services.AddMvcServices(builder.Configuration);

// Build app
var app = builder.Build();

// Configure pipeline
app.ConfigureMvcPipeline();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("APISettings:VariBillAPIAddress = {ApiAddr}", apiAddr);

app.Run();




