using VariBillWebApp.Mvc.Models.ViewModels;

namespace VariBillWebApp.Mvc.Services.Abstractions;

public interface IProductTypeService : IVariBillApiOperations
{
    Task<IReadOnlyList<ProductTypeModel>> GetAllProductTypesAsync();
    Task<ProductTypeModel?> GetProductTypeByIdAsync(Guid id);
    Task<ProductTypeModel?> CreateProductTypeAsync(CreateProductTypeModel model);
    Task<bool> UpdateProductTypeAsync(Guid id, UpdateProductTypeModel model);
    Task<(bool Success, string? ErrorMessage)> DeleteProductTypeAsync(Guid id);
}
