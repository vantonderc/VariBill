using Serilog;

namespace VariBillWebApp.Blazor.Extensions;

/// <summary>
/// Host configuration (Serilog).
/// </summary>
public static class HostBuilderExtensions
{
    public static WebApplicationBuilder ConfigureHost(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((ctx, lc) => lc
            .ReadFrom.Configuration(ctx.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File("logs/blazor-.txt", rollingInterval: RollingInterval.Day));

        return builder;
    }
}