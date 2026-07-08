//using Microsoft.EntityFrameworkCore;
//using VariBillWebAPI.Data.Context;
//using VariBillWebAPI.Data.Entities;
//using VariBillWebAPI.Data.Repository.Interface;

//namespace VariBillWebAPI.Data.Repository;

//public class ProductTypeRepository : Repository<ProductType>, IProductTypeRepository
//{
//    public ProductTypeRepository(VeriBillTestDBContext context, ILogger<ProductTypeRepository> logger)
//        : base(context, logger)
//    {
//    }

//    public async Task<IReadOnlyList<ProductType>> GetAllWithProductCountsAsync()
//    {
//        return await _dbSet
//            .AsNoTracking()
//            .OrderBy(pt => pt.Name)
//            .ToListAsync();
//    }

//    public async Task<ProductType?> GetByIdWithProductsAsync(Guid id)
//    {
//        return await _dbSet
//            .Include(pt => pt.Products)
//            .FirstOrDefaultAsync(pt => pt.Id == id);
//    }

//    public async Task<bool> HasProductsAsync(Guid productTypeId)
//    {
//        return await _context.Products.AnyAsync(p => p.ProductTypeId == productTypeId);
//    }
//}
