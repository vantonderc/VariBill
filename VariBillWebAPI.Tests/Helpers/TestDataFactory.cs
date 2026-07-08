using VariBillWebAPI.Data.Entities;

namespace VariBillWebAPI.Tests.Helpers;

/// <summary>
/// Factory for creating test data.
/// </summary>
public static class TestDataFactory
{
    /// <summary>
    /// Creates a list of test products.
    /// </summary>
    public static List<Product1> CreateTestProducts()
    {
        var productTypeId = Guid.NewGuid();

        return new List<Product1>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Laptop",
                SKU = "LAP-001",
                Price = 1299.99m,
                Quantity = 10,
                Description = "High-performance laptop",
                ProductTypeId = productTypeId,
                IsActive = true,
                IsDeleted = false,
                DateCreated = DateTime.UtcNow.AddDays(-10)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Wireless Mouse",
                SKU = "MOU-002",
                Price = 29.99m,
                Quantity = 50,
                Description = "Ergonomic wireless mouse",
                ProductTypeId = productTypeId,
                IsActive = true,
                IsDeleted = false,
                DateCreated = DateTime.UtcNow.AddDays(-5)
            }
        };
    }

    /// <summary>
    /// Creates a list of test product types.
    /// </summary>
    public static List<ProductType1> CreateTestProductTypes()
    {
        return new List<ProductType1>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Electronics",
                Description = "Electronic devices and gadgets",
                IsActive = true,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Books",
                Description = "Physical and digital books",
                IsActive = true,
                IsDeleted = false
            }
        };
    }
}