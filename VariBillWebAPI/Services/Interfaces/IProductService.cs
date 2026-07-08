using VariBillWebAPI.Models.DTO;

namespace VariBillWebAPI.Services.Abstractions;

/// <summary>
/// Product service interface.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Retrieves all products as DTOs.
    /// </summary>
    Task<IReadOnlyList<ProductResponseDto>> GetAllAsync();

    /// <summary>
    /// Retrieves a specific product DTO by id.
    /// </summary>
    Task<ProductResponseDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Creates a new product from the provided DTO.
    /// </summary>
    Task<ProductResponseDto> CreateAsync(CreateProductDto dto);

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    Task<bool> UpdateAsync(Guid id, UpdateProductDto dto);

    /// <summary>
    /// Soft-deletes a product.
    /// </summary>
    Task<bool> DeleteAsync(Guid id);
}


