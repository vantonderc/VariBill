using VariBillWebAPI.Data.Entities;
using VariBillWebAPI.Data.UnitOfWork.Interfaces;
using VariBillWebAPI.Models.DTO;
using VariBillWebAPI.Services.Abstractions;

namespace VariBillWebAPI.Services;

/// <summary>
/// Product type service implementation.
/// </summary>
/// <summary>
/// Service providing operations for product types.
/// </summary>
/// <seealso cref="VariBillWebAPI.Data.Repository.Interfaces.IProductTypeRepository" />
public class ProductTypeService : IProductTypeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProductTypeService> _logger;

    public ProductTypeService(IUnitOfWork unitOfWork, ILogger<ProductTypeService> logger)
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ProductTypeService"/>.
        /// </summary>
        /// <param name="unitOfWork">Unit of work for data access.</param>
        /// <param name="logger">Logger instance.</param>
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ProductTypeResponseDto>> GetAllAsync()
    {
        var typesWithCounts = await _unitOfWork.ProductTypes.GetAllWithProductCountsAsync();
        return typesWithCounts
            .Select(t => new ProductTypeResponseDto(
                t.ProductType.Id,
                t.ProductType.Name,
                t.ProductType.Description,
                t.Count))
            .ToList();
    }

    public async Task<ProductTypeResponseDto?> GetByIdAsync(Guid id)
    {
        var type = await _unitOfWork.ProductTypes.GetByIdWithProductsAsync(id);
        if (type is null)
            return null;

        return new ProductTypeResponseDto(
            type.Id,
            type.Name,
            type.Description,
            type.Products?.Count(p => !p.IsDeleted) ?? 0);
    }

    public async Task<ProductTypeResponseDto> CreateAsync(CreateProductTypeDto dto)
    {
        var type = new ProductType
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            IsActive = true,
            IsDeleted = false
        };

        await _unitOfWork.ProductTypes.AddAsync(type);
        await _unitOfWork.SaveChangesAsync();

        return new ProductTypeResponseDto(type.Id, type.Name, type.Description, 0);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateProductTypeDto dto)
    {
        var type = await _unitOfWork.ProductTypes.GetByIdAsync(id);
        if (type is null)
            return false;

        type.Name = dto.Name.Trim();
        type.Description = dto.Description?.Trim();
        type.IsActive = dto.IsActive;

        await _unitOfWork.ProductTypes.UpdateAsync(type);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id)
    {
        var type = await _unitOfWork.ProductTypes.GetByIdAsync(id);
        if (type is null)
            return (false, null);

        // Check if any active products still reference this type
        var hasActiveProducts = await _unitOfWork.ProductTypes.HasActiveProductsAsync(id);
        if (hasActiveProducts)
            return (false, "Cannot delete a product type that still has active products.");

        // Soft delete
        type.IsDeleted = true;
        type.IsActive = false;

        await _unitOfWork.ProductTypes.UpdateAsync(type);
        await _unitOfWork.SaveChangesAsync();

        return (true, null);
    }
}




//using VariBillWebAPI.Data.Entities;
//using VariBillWebAPI.Data.UnitOfWork.Interfaces;
//using VariBillWebAPI.Models.DTO;
//using VariBillWebAPI.Services.Interfaces;

//namespace VariBillWebAPI.Services;

//public class ProductTypeService : IProductTypeService
//{
//    private readonly IUnitOfWork _unitOfWork;
//    private readonly ILogger<ProductTypeService> _logger;

//    public ProductTypeService(IUnitOfWork unitOfWork, ILogger<ProductTypeService> logger)
//    {
//        _unitOfWork = unitOfWork;
//        _logger = logger;
//    }

//    public async Task<IReadOnlyList<ProductTypeResponseDto>> GetAllAsync()
//    { // Repository now returns tuples: (ProductType, int)
//        var productTypesWithCounts = await _unitOfWork.ProductTypes.GetAllWithProductCountsAsync();

//        return productTypesWithCounts
//            .Select(tuple => MapToDto(tuple.ProductType, tuple.ProductCount))  // Use tuple.ProductType, tuple.ProductCount
//            .ToList();

//        //var productTypes = await _unitOfWork.ProductTypes.GetAllWithProductCountsAsync();
//        //return productTypes
//        //    .Select(pt => MapToDto(pt, pt.Products?.Count ?? 0))
//        //    .ToList();
//    }

//    public async Task<ProductTypeResponseDto?> GetByIdAsync(Guid id)
//    {
//        var productType = await _unitOfWork.ProductTypes.GetByIdWithProductsAsync(id);
//        return productType is null
//            ? null
//            : MapToDto(productType, productType.Products.Count);
//    }

//    public async Task<ProductTypeResponseDto> CreateAsync(CreateProductTypeDto dto)
//    {
//        var productType = new ProductType
//        {
//            Id = Guid.NewGuid(),
//            Name = dto.Name.Trim(),
//            Description = dto.Description?.Trim()
//        };

//        await _unitOfWork.ProductTypes.AddAsync(productType);
//        await _unitOfWork.SaveChangesAsync();

//        return MapToDto(productType, 0);
//    }

//    public async Task<bool> UpdateAsync(Guid id, UpdateProductTypeDto dto)
//    {
//        var productType = await _unitOfWork.ProductTypes.FindAsync(id);
//        if (productType is null)
//        {
//            return false;
//        }

//        productType.Name = dto.Name.Trim();
//        productType.Description = dto.Description?.Trim();

//        await _unitOfWork.ProductTypes.UpdateAsync(productType);
//        await _unitOfWork.SaveChangesAsync();
//        return true;
//    }

//    public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id)
//    {
//        var productType = await _unitOfWork.ProductTypes.FindAsync(id);
//        if (productType is null)
//        {
//            return (false, null);
//        }

//        if (await _unitOfWork.ProductTypes.HasProductsAsync(id))
//        {
//            return (false, "Cannot delete a product type that still has products.");
//        }

//        await _unitOfWork.ProductTypes.DeleteAsync(productType);
//        await _unitOfWork.SaveChangesAsync();
//        return (true, null);
//    }

//    private static ProductTypeResponseDto MapToDto(ProductType productType, int productCount) =>
//        new(productType.Id, productType.Name, productType.Description, productCount);
//}
