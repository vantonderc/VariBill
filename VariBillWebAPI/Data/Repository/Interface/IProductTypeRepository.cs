
using VariBillWebAPI.Data.Entities;
using VariBillWebAPI.Data.Repository.Interfaces;

namespace VariBillWebAPI.Data.Repository.Interfaces;

/// <summary>
/// Product type repository interface.
/// </summary>
public interface IProductTypeRepository : IRepository<ProductType>
{
    /// <summary>
    /// Gets product types along with counts of active products for each type.
    /// </summary>
    Task<IReadOnlyList<(ProductType ProductType, int Count)>> GetAllWithProductCountsAsync();

    /// <summary>
    /// Gets a product type by id and includes its related products.
    /// </summary>
    Task<ProductType?> GetByIdWithProductsAsync(Guid id);

    /// <summary>
    /// Determines whether the product type has any active products.
    /// </summary>
    Task<bool> HasActiveProductsAsync(Guid productTypeId);
}


