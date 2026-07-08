using Microsoft.EntityFrameworkCore;
using VariBillWebAPI.Data.Context;
using VariBillWebAPI.Data.Entities;

namespace VariBillWebAPI.Data.Seeds;

/// <summary>
/// Seeds the database with initial data.
/// </summary>
public class DBContextSeedData
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DBContextSeedData> _logger;

    public DBContextSeedData(IServiceProvider serviceProvider, ILogger<DBContextSeedData> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task SeedAllAsync()
    {
        try
        {
            _logger.LogInformation("Starting database seeding...");

            await SeedProductTypeData();
            await SeedProductData();

            _logger.LogInformation("Database seeding completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding database.");
            throw;
        }
    }

    private async Task SeedProductTypeData()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<VeriBillTestDBContext>();

        if (await context.ProductTypes.AnyAsync())
            return;

        var productTypes = new List<ProductType>
        {
            new() { Id = Guid.NewGuid(), Name = "Electronics", Description = "Electronic devices and gadgets" },
            new() { Id = Guid.NewGuid(), Name = "Books", Description = "Physical and digital books" },
            new() { Id = Guid.NewGuid(), Name = "Clothing", Description = "Apparel and accessories" },
            new() { Id = Guid.NewGuid(), Name = "Home & Garden", Description = "Furniture and home decor" },
            new() { Id = Guid.NewGuid(), Name = "Sports & Outdoors", Description = "Sports equipment and outdoor gear" }
        };

        await context.ProductTypes.AddRangeAsync(productTypes);
        await context.SaveChangesAsync();

        _logger.LogInformation("Seeded {Count} product types.", productTypes.Count);
    }

    private async Task SeedProductData()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<VeriBillTestDBContext>();

        if (await context.Products.AnyAsync())
            return;

        var types = await context.ProductTypes.ToListAsync();
        if (!types.Any())
        {
            _logger.LogWarning("No product types found; skipping product seeding.");
            return;
        }

        var electronics = types.First(t => t.Name == "Electronics");
        var books = types.First(t => t.Name == "Books");
        var clothing = types.First(t => t.Name == "Clothing");
        var home = types.First(t => t.Name == "Home & Garden");
        var sports = types.First(t => t.Name == "Sports & Outdoors");

        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), ProductTypeId = electronics.Id, Name = "Laptop", SKU = "LAP-001", Price = 1299.99m, Quantity = 10, Description = "High-performance laptop", IsActive = true },
            new() { Id = Guid.NewGuid(), ProductTypeId = electronics.Id, Name = "Wireless Mouse", SKU = "MOU-002", Price = 29.99m, Quantity = 50, Description = "Ergonomic wireless mouse", IsActive = true },
            new() { Id = Guid.NewGuid(), ProductTypeId = books.Id, Name = "Clean Code", SKU = "BOK-003", Price = 45.00m, Quantity = 8, Description = "A Handbook of Agile Software Craftsmanship", IsActive = true },
            new() { Id = Guid.NewGuid(), ProductTypeId = books.Id, Name = "Design Patterns", SKU = "BOK-004", Price = 55.00m, Quantity = 5, Description = "Elements of Reusable Object-Oriented Software", IsActive = true },
            new() { Id = Guid.NewGuid(), ProductTypeId = clothing.Id, Name = "Cotton T-Shirt", SKU = "CLO-005", Price = 19.99m, Quantity = 100, Description = "100% organic cotton", IsActive = true },
            new() { Id = Guid.NewGuid(), ProductTypeId = home.Id, Name = "Desk Lamp", SKU = "HOM-006", Price = 39.99m, Quantity = 20, Description = "LED desk lamp", IsActive = true },
            new() { Id = Guid.NewGuid(), ProductTypeId = sports.Id, Name = "Running Shoes", SKU = "SPO-007", Price = 129.99m, Quantity = 15, Description = "Professional running shoes", IsActive = true }
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();

        _logger.LogInformation("Seeded {Count} products.", products.Count);
    }
}






