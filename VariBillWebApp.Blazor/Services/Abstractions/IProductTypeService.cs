using VariBillWebApp.Blazor.Models.DTOs;

namespace VariBillWebApp.Blazor.Services.Abstractions;

/// <summary>
/// Product type service interface.
/// </summary>
public interface IProductTypeService
{
    Task<List<ProductTypeApiResponseDto>> GetAllProductTypesAsync();
    Task<ProductTypeApiResponseDto?> GetProductTypeByIdAsync(Guid id);
    Task<ProductTypeApiResponseDto?> CreateProductTypeAsync(CreateProductTypeDto dto);
    Task<bool> UpdateProductTypeAsync(Guid id, UpdateProductTypeDto dto);
    Task<(bool Success, string? ErrorMessage)> DeleteProductTypeAsync(Guid id);
}