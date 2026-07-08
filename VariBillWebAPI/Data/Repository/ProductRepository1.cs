

//using Microsoft.EntityFrameworkCore;
//using VariBillWebAPI.Data.Context;
//using VariBillWebAPI.Data.Entities;
//using VariBillWebAPI.Data.Repository.Interface;

//namespace VariBillWebAPI.Data.Repository;

////TODO:ensure all areas correctly renamed from profile to product etc..
////TODO:ensure all the TODO comments removed when done
////TODO:ensure all program.cs services/repos/etc added and all middleware added..!!!
////TODO:ensure constants used where needed and nameof/typeof !!! ->check DT
////TODO: check DT for all Blazor related things as well as things like Antiforgery token etc..
////TODO(NB): use hardcoded test values if still struggle with sqlserver...and sqlite not an option...
////....NB->maybe check moq....
////TODO:NB->check BONUS quesion explicit/lazy/etc....
//public class ProductRepository : Repository<Product>, IProductRepository
//{
//    private readonly VeriBillTestDBContext _ctx;
//    private readonly ILogger<ProductRepository> _typedLogger; //TODO:why typedLogger? and not logger?

//    public ProductRepository(VeriBillTestDBContext context, ILogger<ProductRepository> logger)
//        : base(context, logger)
//    {
//        _ctx = context;
//        _typedLogger = logger;
//    }

//    public async Task<IReadOnlyList<Product>> GetAllWithProductTypeAsync()
//    {
//        return await _dbSet
//            .AsNoTracking()
//            .Include(p => p.ProductType)
//            .OrderBy(p => p.Name)
//            .ToListAsync();
//    }



//    public async Task<Product?> GetByIdWithProductTypeAsync(Guid id)
//    {
//        return await _dbSet
//            .Include(p => p.ProductType)
//            .FirstOrDefaultAsync(p => p.Id == id);
//    }

    

//        //TODO:add filtering limits etc. ....later 
//    public async Task<IEnumerable<Product>> GetAllProductsAsync()//(string excludeProfileBID)//TODO:enumaber
//    {
//        try
//        {
//            //    var products = await _context.Products
//            //        .Include(p => p.ProductType)  // Eager loading
//            //        .Select(p => new ProductDto(p.Id, p.Name, p.Price,
//            //            p.Description, p.ProductTypeId, p.ProductType!.Name))
//            //        .ToListAsync();

//            return await _context.Products
//                    .Include(p => p.ProductType) // Eager Loading
//                    .ToListAsync(); //TODO:when to use tolsit  here or only higher up

            
//        }
//        catch (Exception ex)
//        {
//            _typedLogger.LogError(ex, "Error in GetAllProductAsync");
//            throw;
//        }
//    }

    
//}
