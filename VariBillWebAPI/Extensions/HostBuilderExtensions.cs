//using Serilog;

//namespace VariBillWebAPI.Extensions;

///// <summary>
///// Host configuration (Serilog).
///// </summary>
//public static class HostBuilderExtensions
//{
//    public static WebApplicationBuilder ConfigureHost(this WebApplicationBuilder builder)
//    {
//        builder.Host.UseSerilog((ctx, lc) => lc
//            .ReadFrom.Configuration(ctx.Configuration)
//            .Enrich.FromLogContext()
//           // .Enrich.WithMachineName()
//           // .Enrich.WithThreadId()
//            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
//            .WriteTo.File("logs/api-.txt", rollingInterval: RollingInterval.Day)
//            .Filter.ByExcluding(logEvent =>
//                logEvent.Properties.TryGetValue("RequestPath", out var path) &&
//                path.ToString().Contains("/health")));

//        return builder;
//    }
//}
