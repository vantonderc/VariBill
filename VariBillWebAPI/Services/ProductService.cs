using VariBillWebAPI.Data.Entities;
using VariBillWebAPI.Data.UnitOfWork.Interfaces;
using VariBillWebAPI.Exceptions;
using VariBillWebAPI.Models.DTO;
using VariBillWebAPI.Services.Abstractions;

namespace VariBillWebAPI.Services;

/// <summary>
/// Product service implementation.
/// </summary>
public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly ILogger<ProductService> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="ProductService"/>.
    /// </summary>
    /// <param name="unitOfWork">Unit of work for repository access.</param>
    /// <param name="cacheService">Cache service used to cache product lists.</param>
    /// <param name="logger">Logger instance.</param>
    public ProductService(IUnitOfWork unitOfWork, ICacheService cacheService, ILogger<ProductService> logger)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ProductResponseDto>> GetAllAsync()
    {
        /// <summary>
        /// Retrieves all products including their product type name. Results are cached for a short period.
        /// </summary>
        const string cacheKey = "all_products";
        var cached = await _cacheService.GetAsync<IReadOnlyList<ProductResponseDto>>(cacheKey);
        if (cached != null)
            return cached;

        var products = await _unitOfWork.Products.GetAllWithProductTypeAsync();
        var result = products.Select(MapToDto).ToList();

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
        return result;
    }

    public async Task<ProductResponseDto?> GetByIdAsync(Guid id)
    {
        var product = await _unitOfWork.Products.GetByIdWithProductTypeAsync(id);
        return product is null ? null : MapToDto(product);
    }

    public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto)
    {
        // Validate ProductType exists
        if (!await ProductTypeExistsAsync(dto.ProductTypeId))
            throw new Exceptions.ValidationException($"Product type '{dto.ProductTypeId}' was not found.");

        // Validate SKU uniqueness
        IReadOnlyList<Product> existing = await _unitOfWork.Products.FindAsync(p => p.SKU == dto.SKU);
        if (existing != null && existing.Any())
            throw new BusinessRuleException($"A product with SKU '{dto.SKU}' already exists.");

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            SKU = dto.SKU.Trim(),
            Price = dto.Price,
            Quantity = dto.Quantity,
            Description = dto.Description?.Trim(),
            ProductTypeId = dto.ProductTypeId,
            DateCreated = DateTime.UtcNow,
            IsActive = true,
            IsDeleted = false
        };

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        // Invalidate cache
        await _cacheService.RemoveAsync("all_products");

        var created = await _unitOfWork.Products.GetByIdWithProductTypeAsync(product.Id) ?? product;
        return MapToDto(created);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateProductDto dto)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product is null)
            return false;

        // Validate ProductType exists
        if (!await ProductTypeExistsAsync(dto.ProductTypeId))
            throw new Exceptions.ValidationException($"Product type '{dto.ProductTypeId}' was not found.");

        // Validate SKU uniqueness (excluding self)
        var duplicate = await _unitOfWork.Products.FindAsync(p => p.SKU == dto.SKU && p.Id != id);
        if (duplicate.Any())
            throw new BusinessRuleException($"A product with SKU '{dto.SKU}' already exists.");

        product.Name = dto.Name.Trim();
        product.SKU = dto.SKU.Trim();
        product.Price = dto.Price;
        product.Quantity = dto.Quantity;
        product.Description = dto.Description?.Trim();
        product.ProductTypeId = dto.ProductTypeId;
        product.IsActive = dto.IsActive;
        product.DateModified = DateTime.UtcNow;

        await _unitOfWork.Products.UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync();

        // Invalidate cache
        await _cacheService.RemoveAsync("all_products");

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product is null)
            return false;

        // Soft delete
        product.IsDeleted = true;
        product.IsActive = false;
        product.DateModified = DateTime.UtcNow;

        await _unitOfWork.Products.UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync();

        // Invalidate cache
        await _cacheService.RemoveAsync("all_products");

        return true;
    }

    private async Task<bool> ProductTypeExistsAsync(Guid productTypeId)
    {
        return await _unitOfWork.ProductTypes.GetByIdAsync(productTypeId) is not null;
    }

    private static ProductResponseDto MapToDto(Product product) =>
        new(
            product.Id,
            product.Name,
            product.SKU,
            product.Price,
            product.Quantity,
            product.Description,
            product.ProductTypeId,
            product.ProductType?.Name,
            product.IsActive,
            product.DateCreated,
            product.DateModified);
}



