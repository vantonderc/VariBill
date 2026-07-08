using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using VariBillWebAPI.Data.Entities;
using VariBillWebAPI.Data.Repository.Interface;
using VariBillWebAPI.Data.UnitOfWork.Interfaces;
using VariBillWebAPI.Models.DTO;
using VariBillWebAPI.Services;
using VariBillWebAPI.Tests.Helpers;
using Xunit;

namespace VariBillWebAPI.Tests.Services;

/// <summary>
/// Unit tests for <see cref="ProductService"/>.
/// </summary>
public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IProductRepository> _productRepoMock;
    private readonly Mock<IProductTypeRepository> _productTypeRepoMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly Mock<ILogger<ProductService>> _loggerMock;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productRepoMock = new Mock<IProductRepository>();
        _productTypeRepoMock = new Mock<IProductTypeRepository>();
        _cacheServiceMock = new Mock<ICacheService>();
        _loggerMock = new Mock<ILogger<ProductService>>();

        _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.ProductTypes).Returns(_productTypeRepoMock.Object);

        _service = new ProductService(
            _unitOfWorkMock.Object,
            _cacheServiceMock.Object,
            _loggerMock.Object);
    }

    // ==================== GetAllAsync Tests ====================

    [Fact]
    public async Task GetAllAsync_WhenCacheHasData_ReturnsCachedData()
    {
        // Arrange
        var cacheKey = "all_products";
        var cachedProducts = new List<ProductResponseDto>
        {
            new(Guid.NewGuid(), "Cached Laptop", "LAP-001", 1299.99m, 10, null, Guid.NewGuid(), "Electronics", true, DateTime.UtcNow, null)
        };

        _cacheServiceMock
            .Setup(c => c.GetAsync<IReadOnlyList<ProductResponseDto>>(cacheKey))
            .ReturnsAsync(cachedProducts);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Cached Laptop", result[0].Name);
        _productRepoMock.Verify(r => r.GetAllWithProductTypeAsync(), Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_WhenCacheEmpty_ReturnsProductsFromRepository()
    {
        // Arrange
        var products = TestDataFactory.CreateTestProducts();

        _cacheServiceMock
            .Setup(c => c.GetAsync<IReadOnlyList<ProductResponseDto>>("all_products"))
            .ReturnsAsync((IReadOnlyList<ProductResponseDto>?)null);

        _productRepoMock
            .Setup(r => r.GetAllWithProductTypeAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Laptop", result[0].Name);
        Assert.Equal("Wireless Mouse", result[1].Name);

        _cacheServiceMock.Verify(c => c.SetAsync("all_products", It.IsAny<IReadOnlyList<ProductResponseDto>>(), It.IsAny<TimeSpan>()), Times.Once);
    }

    // ==================== GetByIdAsync Tests ====================

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ReturnsProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product1
        {
            Id = productId,
            Name = "Test Product",
            SKU = "TEST-001",
            Price = 99.99m,
            Quantity = 5,
            ProductTypeId = Guid.NewGuid(),
            ProductType = new ProductType1 { Name = "Test Type" },
            IsActive = true,
            DateCreated = DateTime.UtcNow
        };

        _productRepoMock
            .Setup(r => r.GetByIdWithProductTypeAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _service.GetByIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal("Test Product", result.Name);
        Assert.Equal("Test Type", result.ProductTypeName);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ReturnsNull()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _productRepoMock
            .Setup(r => r.GetByIdWithProductTypeAsync(productId))
            .ReturnsAsync((Product1?)null);

        // Act
        var result = await _service.GetByIdAsync(productId);

        // Assert
        Assert.Null(result);
    }

    // ==================== CreateAsync Tests ====================

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsCreatedProduct()
    {
        // Arrange
        var productTypeId = Guid.NewGuid();
        var dto = new CreateProductDto(
            "New Product",
            "SKU-001",
            99.99m,
            10,
            "Test description",
            productTypeId);

        var productType = new ProductType1 { Id = productTypeId, Name = "Test Type" };

        _productTypeRepoMock
            .Setup(r => r.GetByIdAsync(productTypeId))
            .ReturnsAsync(productType);

        _productRepoMock
            .Setup(r => r.FindAsync(It.IsAny<Expression<Func<Product1, bool>>>()))
            .ReturnsAsync(new List<Product1>());

        _productRepoMock
             .Setup(r => r.AddAsync(It.IsAny<Product1>()))
             .ReturnsAsync((Product1 p) =>
             {
                 p.Id = Guid.NewGuid(); // Simulate the service setting an ID
                 return p;
             });

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Create a specific product to return
        var createdProduct = new Product1
        {
            Id = Guid.NewGuid(),
            Name = "New Product",
            SKU = "SKU-001",
            Price = 99.99m,
            Quantity = 10,
            Description = "Test description",
            ProductTypeId = productTypeId,
            ProductType = productType,
            IsActive = true,
            DateCreated = DateTime.UtcNow
        };

        _productRepoMock
            .Setup(r => r.GetByIdWithProductTypeAsync(It.IsAny<Guid>()))
            .ReturnsAsync(createdProduct);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Product", result.Name);
        Assert.Equal("SKU-001", result.SKU);
        Assert.Equal(99.99m, result.Price);
        Assert.Equal("Test Type", result.ProductTypeName);

        _cacheServiceMock.Verify(c => c.RemoveAsync("all_products"), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenProductTypeNotFound_ThrowsValidationException()
    {
        // Arrange
        var dto = new CreateProductDto(
            "New Product",
            "SKU-001",
            99.99m,
            10,
            null,
            Guid.NewGuid());

        _productTypeRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((ProductType1?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => _service.CreateAsync(dto));

        Assert.Contains("Product type", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenDuplicateSKU_ThrowsBusinessRuleException()
    {
        // Arrange
        var productTypeId = Guid.NewGuid();
        var dto = new CreateProductDto(
            "New Product",
            "SKU-001",
            99.99m,
            10,
            null,
            productTypeId);

        var productType = new ProductType1 { Id = productTypeId };
        var existingProducts = new List<Product1>
        {
            new() { Id = Guid.NewGuid(), SKU = "SKU-001" }
        };

        _productTypeRepoMock
            .Setup(r => r.GetByIdAsync(productTypeId))
            .ReturnsAsync(productType);

        _productRepoMock
            .Setup(r => r.FindAsync(It.IsAny<Expression<Func<Product1, bool>>>()))
            .ReturnsAsync(existingProducts);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.CreateAsync(dto));

        Assert.Contains("SKU", exception.Message);
    }

    // ==================== UpdateAsync Tests ====================

    [Fact]
    public async Task UpdateAsync_WithValidData_ReturnsTrue()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productTypeId = Guid.NewGuid();
        var product = new Product1
        {
            Id = productId,
            Name = "Old Name",
            SKU = "OLD-001",
            Price = 50.00m,
            Quantity = 5,
            ProductTypeId = Guid.NewGuid()
        };

        var dto = new UpdateProductDto(
            productId,
            "Updated Name",
            "NEW-001",
            75.00m,
            10,
            "Updated description",
            productTypeId,
            true);

        var productType = new ProductType1 { Id = productTypeId };

        _productRepoMock
            .Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _productTypeRepoMock
            .Setup(r => r.GetByIdAsync(productTypeId))
            .ReturnsAsync(productType);

        _productRepoMock
            .Setup(r => r.FindAsync(It.IsAny<Expression<Func<Product1, bool>>>()))
            .ReturnsAsync(new List<Product1>());

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.UpdateAsync(productId, dto);

        // Assert
        Assert.True(result);
        Assert.Equal("Updated Name", product.Name);
        Assert.Equal("NEW-001", product.SKU);
        Assert.Equal(75.00m, product.Price);
        Assert.Equal(10, product.Quantity);
        Assert.Equal(productTypeId, product.ProductTypeId);
        Assert.True(product.IsActive);
        Assert.NotNull(product.DateModified);

        _cacheServiceMock.Verify(c => c.RemoveAsync("all_products"), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenProductNotFound_ReturnsFalse()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var dto = new UpdateProductDto(
            productId,
            "Name",
            "SKU-001",
            99.99m,
            10,
            null,
            Guid.NewGuid(),
            true);

        _productRepoMock
            .Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync((Product1?)null);

        // Act
        var result = await _service.UpdateAsync(productId, dto);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenDuplicateSKU_ThrowsBusinessRuleException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productTypeId = Guid.NewGuid();
        var product = new Product1
        {
            Id = productId,
            Name = "Old Name",
            SKU = "OLD-001",
            Price = 50.00m,
            Quantity = 5,
            ProductTypeId = Guid.NewGuid()
        };

        var dto = new UpdateProductDto(
            productId,
            "Updated Name",
            "DUPLICATE-001",
            75.00m,
            10,
            null,
            productTypeId,
            true);

        var productType = new ProductType1 { Id = productTypeId };
        var duplicateProducts = new List<Product1>
        {
            new() { Id = Guid.NewGuid(), SKU = "DUPLICATE-001" }
        };

        _productRepoMock
            .Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _productTypeRepoMock
            .Setup(r => r.GetByIdAsync(productTypeId))
            .ReturnsAsync(productType);

        _productRepoMock
            .Setup(r => r.FindAsync(It.IsAny<Expression<Func<Product1, bool>>>()))
            .ReturnsAsync(duplicateProducts);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.UpdateAsync(productId, dto));

        Assert.Contains("SKU", exception.Message);
    }

    // ==================== DeleteAsync Tests ====================

    [Fact]
    public async Task DeleteAsync_WhenProductExists_SoftDeletesAndReturnsTrue()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product1
        {
            Id = productId,
            Name = "Test Product",
            IsDeleted = false,
            IsActive = true,
            DateModified = null
        };

        _productRepoMock
            .Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.DeleteAsync(productId);

        // Assert
        Assert.True(result);
        Assert.True(product.IsDeleted);
        Assert.False(product.IsActive);
        Assert.NotNull(product.DateModified);

        _cacheServiceMock.Verify(c => c.RemoveAsync("all_products"), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductNotFound_ReturnsFalse()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _productRepoMock
            .Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync((Product1?)null);

        // Act
        var result = await _service.DeleteAsync(productId);

        // Assert
        Assert.False(result);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
}
