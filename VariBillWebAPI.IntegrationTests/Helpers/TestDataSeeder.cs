using VariBillWebAPI.Data.Context;
using VariBillWebAPI.Data.Entities;

namespace VariBillWebAPI.IntegrationTests.Helpers;

/// <summary>
/// Seeds the in‑memory database with test data.
/// </summary>
public static class TestDataSeeder
{
    public static void Seed(VeriBillTestDBContext context)
    {
        // ProductTypes
        var electronics = new ProductType1
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "Electronics",
            Description = "Electronic devices",
            IsActive = true,
            IsDeleted = false
        };

        var books = new ProductType1
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Name = "Books",
            Description = "Physical and digital books",
            IsActive = true,
            IsDeleted = false
        };

        context.ProductTypes.AddRange(electronics, books);

        // Products
        context.Products.AddRange(
            new Product
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                ProductTypeId = electronics.Id,
                Name = "Laptop",
                SKU = "LAP-001",
                Price = 1299.99m,
                Quantity = 10,
                Description = "High-performance laptop",
                IsActive = true,
                IsDeleted = false,
                DateCreated = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                ProductTypeId = books.Id,
                Name = "Clean Code",
                SKU = "BOK-001",
                Price = 45.00m,
                Quantity = 8,
                Description = "A Handbook of Agile Software Craftsmanship",
                IsActive = true,
                IsDeleted = false,
                DateCreated = DateTime.UtcNow
            }
        );

        context.SaveChanges();
    }
}
