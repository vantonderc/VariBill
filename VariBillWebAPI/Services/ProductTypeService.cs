using VariBillWebAPI.Data.Entities;
using VariBillWebAPI.Data.UnitOfWork.Interfaces;
using VariBillWebAPI.Models.DTO;
using VariBillWebAPI.Services.Abstractions;


namespace VariBillWebAPI.Services;

/// <summary>
/// Service providing operations for product types.
/// </summary>
public class ProductTypeService : IProductTypeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProductTypeService> _logger;

    public ProductTypeService(IUnitOfWork unitOfWork, ILogger<ProductTypeService> logger)
    {
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
