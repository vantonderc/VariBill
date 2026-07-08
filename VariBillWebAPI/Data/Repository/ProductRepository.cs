
using Microsoft.EntityFrameworkCore;
using VariBillWebAPI.Data.Context;
using VariBillWebAPI.Data.Entities;
using VariBillWebAPI.Data.Repository.Interfaces;

namespace VariBillWebAPI.Data.Repository;

/// <summary>
/// Product repository implementation.
/// </summary>
public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(VeriBillTestDBContext context, ILogger<ProductRepository> logger)
        : base(context, logger)
    {
    }

    public async Task<IReadOnlyList<Product>> GetAllWithProductTypeAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Include(p => p.ProductType)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdWithProductTypeAsync(Guid id)
    {
        return await _dbSet
            .Include(p => p.ProductType)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}

