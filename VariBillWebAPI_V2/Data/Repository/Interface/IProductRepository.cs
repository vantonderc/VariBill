using VariBillWebAPI.Data.Entities;
using VariBillWebAPI.Data.Repository.Interfaces;

namespace VariBillWebAPI.Data.Repository.Interfaces;

/// <summary>
/// Product repository interface.
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    /// <summary>
    /// Gets all products and includes their product type navigation property.
    /// </summary>
    Task<IReadOnlyList<Product>> GetAllWithProductTypeAsync();

    /// <summary>
    /// Gets a product by id and includes its product type navigation property.
    /// </summary>
    Task<Product?> GetByIdWithProductTypeAsync(Guid id);
}





