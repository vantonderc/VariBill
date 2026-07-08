using VariBillWebApp.Blazor.Models.DTOs;
using VariBillWebApp.Blazor.Models.ViewModels;

namespace VariBillWebApp.Blazor.Services.Abstractions;

/// <summary>
/// Product service interface.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Retrieves all products from the API.
    /// </summary>
    Task<List<ProductApiResponseDto>> GetAllProductsAsync();

    /// <summary>
    /// Retrieves a product by identifier from the API.
    /// </summary>
    Task<ProductApiResponseDto?> GetProductByIdAsync(Guid id);

    /// <summary>
    /// Creates a new product via the API.
    /// </summary>
    Task<ProductApiResponseDto?> CreateProductAsync(ProductFormViewModel model);

    /// <summary>
    /// Updates an existing product via the API.
    /// </summary>
    Task<bool> UpdateProductAsync(Guid id, ProductFormViewModel model);

    /// <summary>
    /// Deletes a product via the API.
    /// </summary>
    Task<bool> DeleteProductAsync(Guid id);
}
