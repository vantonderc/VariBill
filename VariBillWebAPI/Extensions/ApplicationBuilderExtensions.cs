//using Microsoft.EntityFrameworkCore;
//using Serilog;
//using VariBillWebAPI.Data.Context;
//using VariBillWebAPI.Data.Seeds;
//using VariBillWebAPI.Middleware;

//namespace VariBillWebAPI.Extensions;

///// <summary>
///// Middleware pipeline configuration.
///// </summary>
//public static class ApplicationBuilderExtensions
//{
//    /// <summary>
//    /// Configures the API middleware pipeline.
//    /// </summary>
//    public static WebApplication ConfigureApiPipeline(this WebApplication app)
//    {
//        app.UseSerilogRequestLogging();

//        // Global exception handling
//        app.UseMiddleware<GlobalExceptionMiddleware>();

//        // Development specific
//        if (app.Environment.IsDevelopment())
//        {
//           app.UseSwagger();
//           app.UseSwaggerUI();
//            app.UseDeveloperExceptionPage();
//        }
//        else
//        {
//            app.UseHsts();
//        }

//        app.UseHttpsRedirection();
//        app.UseResponseCaching();
//        app.UseCors("AllowAll");

//        app.UseRouting();

//        app.UseAuthentication();
//        app.UseAuthorization();

//        // Health checks
//        app.MapHealthChecks("/health");

//        // Controllers
//        app.MapControllers();

//        // Seed data (development only)
//        if (app.Environment.IsDevelopment())
//        {
//            using var scope = app.Services.CreateScope();
//            var seed = scope.ServiceProvider.GetRequiredService<DBContextSeedData>();
//            seed.SeedAllAsync().GetAwaiter().GetResult();
//        }

//        // Apply migrations on startup (optional)
//        using (var scope = app.Services.CreateScope())
//        {
//            var db = scope.ServiceProvider.GetRequiredService<VeriBillTestDBContext>();
//            db.Database.Migrate();
//        }

//        return app;
//    }
//}
