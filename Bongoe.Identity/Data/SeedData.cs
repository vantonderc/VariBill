using Microsoft.AspNetCore.Identity;
using Bongoe.Identity.Models;

namespace Bongoe.Identity.Data;

/// <summary>
/// Seeds the database with default roles and an admin user.
/// </summary>
public static class SeedData
{
    private const string AdminRole = "Admin";
    private const string UserRole = "User";
    private const string DefaultAdminEmail = "admin@varibill.local";
    private const string DefaultAdminPassword = "Admin#12345";

    /// <summary>
    /// Ensures that the required roles and the default admin user exist.
    /// </summary>
    public static async Task EnsureSeedDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // Create roles
        foreach (var role in new[] { AdminRole, UserRole })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Create default admin user
        var admin = await userManager.FindByEmailAsync(DefaultAdminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = DefaultAdminEmail,
                Email = DefaultAdminEmail,
                DisplayName = "VariBill Admin",
                EmailConfirmed = true,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(admin, DefaultAdminPassword);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to seed admin user: {string.Join("; ", result.Errors.Select(e => e.Description))}");
            }
        }

        // Ensure admin has the Admin role
        if (!await userManager.IsInRoleAsync(admin, AdminRole))
        {
            await userManager.AddToRoleAsync(admin, AdminRole);
        }
    }
}