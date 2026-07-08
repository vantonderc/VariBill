using VariBillWebAPI.Models.DTO;

namespace VariBillWebAPI.Services.Abstractions;

/// <summary>
/// Product type service interface.
/// </summary>
public interface IProductTypeService
{
    /// <summary>
    /// Gets all product types with counts.
    /// </summary>
    Task<IReadOnlyList<ProductTypeResponseDto>> GetAllAsync();

    /// <summary>
    /// Gets a product type DTO by identifier.
    /// </summary>
    Task<ProductTypeResponseDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Creates a new product type from the DTO.
    /// </summary>
    Task<ProductTypeResponseDto> CreateAsync(CreateProductTypeDto dto);

    /// <summary>
    /// Updates an existing product type.
    /// </summary>
    Task<bool> UpdateAsync(Guid id, UpdateProductTypeDto dto);

    /// <summary>
    /// Deletes a product type (soft-delete) if no active products exist.
    /// </summary>
    Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id);
}

