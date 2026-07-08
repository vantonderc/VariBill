using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using VariBillWebAPI.Data.Context;
using VariBillWebAPI.IntegrationTests.Helpers;

namespace VariBillWebAPI.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // 1. Tell the factory where the API project is
        builder.UseSolutionRelativeContentRoot("VariBillWebAPI");

        builder.ConfigureServices(services =>
        {
            // 2. Replace real DbContext with in-memory
            services.RemoveAll(typeof(DbContextOptions<VeriBillTestDBContext>));
            services.AddDbContext<VeriBillTestDBContext>(options =>
                options.UseInMemoryDatabase("TestDb"));

            // 3. Add test authentication (always Admin)
            services.AddAuthentication("TestScheme")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });

            // 4. Keep Admin policy
            services.Configure<AuthorizationOptions>(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            });

            // 5. Build and seed data (guarded to avoid test startup crashes)
            var sp = services.BuildServiceProvider();
            try
            {
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<VeriBillTestDBContext>();
                try
                {
                    // Ensure database created; for some providers EnsureDeleted may throw during test host initialization
                    db.Database.EnsureDeleted();
                }
                catch
                {
                    // ignore
                }

                try
                {
                    db.Database.EnsureCreated();
                }
                catch
                {
                    // ignore
                }

                try
                {
                    TestDataSeeder.Seed(db);
                }
                catch
                {
                    // ignore seeding errors in test initialization
                }
            }
            catch
            {
                // ignore service provider / seeding errors to allow tests to continue and report failures
            }
        });
    }
}
