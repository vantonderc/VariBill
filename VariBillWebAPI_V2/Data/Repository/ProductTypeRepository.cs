using Microsoft.EntityFrameworkCore;
using VariBillWebAPI.Data.Context;
using VariBillWebAPI.Data.Entities;
using VariBillWebAPI.Data.Repository.Interfaces;

namespace VariBillWebAPI.Data.Repository;

/// <summary>
/// Product type repository implementation.
/// </summary>
public class ProductTypeRepository : Repository<ProductType>, IProductTypeRepository
{
    public ProductTypeRepository(VeriBillTestDBContext context, ILogger<ProductTypeRepository> logger)
        : base(context, logger)
    {
    }

    public async Task<IReadOnlyList<(ProductType ProductType, int Count)>> GetAllWithProductCountsAsync()
    {
        var results = await _dbSet
            .AsNoTracking()
            .Select(pt => new
            {
                ProductType = pt,
                Count = _context.Products.Count(p => p.ProductTypeId == pt.Id && !p.IsDeleted)
            })
            .OrderBy(x => x.ProductType.Name)
            .ToListAsync();

        return results.Select(x => (x.ProductType, x.Count)).ToList();
    }

    public async Task<ProductType?> GetByIdWithProductsAsync(Guid id)
    {
        return await _dbSet
            .Include(pt => pt.Products.Where(p => !p.IsDeleted))
            .FirstOrDefaultAsync(pt => pt.Id == id);
    }

    public async Task<bool> HasActiveProductsAsync(Guid productTypeId)
    {
        return await _context.Products
            .AnyAsync(p => p.ProductTypeId == productTypeId && !p.IsDeleted);
    }
}







