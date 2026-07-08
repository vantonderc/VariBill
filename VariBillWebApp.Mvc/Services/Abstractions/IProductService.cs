using VariBillWebApp.Mvc.Models.ViewModels;

namespace VariBillWebApp.Mvc.Services.Abstractions;

/// <summary>
/// Product service interface.
/// </summary>
public interface IProductService : IVariBillApiOperations
{
    Task<IReadOnlyList<ProductViewModel>> GetAllProductsAsync();
    Task<ProductViewModel?> GetProductByIdAsync(Guid id);
    Task<ProductViewModel?> CreateProductAsync(ProductFormViewModel model);
    Task<bool> UpdateProductAsync(Guid id, ProductEditViewModel model);
    Task<bool> DeleteProductAsync(Guid id);
}
