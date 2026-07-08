//using Bongoe.Identity;

//using Serilog;
//using Log = Serilog.Log;

//Log.Logger = new LoggerConfiguration()
//    .WriteTo.Console()
//    .CreateBootstrapLogger();

////TODO:moet op login/register baldsy OAuth/OpenId gebruik - dalk op Blazor pages


//try
//{
//    var builder = WebApplication.CreateBuilder(args);

//    // 1. Configure the builder
//    builder.ConfigureLogging();

//    // 2. Configure services and BUILD the app (returns WebApplication)
//    var app = builder.ConfigureServices();

//    // 3. Configure the middleware on the app
//    app.ConfigurePipeline();


//    app.Run();
//}
//catch (Exception ex)
//{
//    Log.Fatal(ex, "Unhandled exception");
//}
//finally
//{
//    Log.Information("Shut down complete");
//    Log.CloseAndFlush();
//}
