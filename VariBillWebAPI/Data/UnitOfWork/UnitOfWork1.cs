//using Microsoft.EntityFrameworkCore.Storage;
//using VariBillWebAPI.Data.Context;
//using VariBillWebAPI.Data.Repository.Interface;
//using VariBillWebAPI.Data.UnitOfWork.Interfaces;

//namespace VariBillWebAPI.Data.UnitOfWork;

//public class UnitOfWork : IUnitOfWork
//{
//    private readonly VeriBillTestDBContext _context;
//    private readonly ILogger<UnitOfWork> _logger;
//    private IDbContextTransaction? _transaction;

//    public IProductRepository Products { get; }
//    public IProductTypeRepository ProductTypes { get; }

//    public UnitOfWork(
//        VeriBillTestDBContext context,
//        ILogger<UnitOfWork> logger,
//        IProductRepository productRepository,
//        IProductTypeRepository productTypeRepository)
//    {
//        _context = context;
//        _logger = logger;
//        Products = productRepository;
//        ProductTypes = productTypeRepository;
//    }

//    public async Task<int> SaveChangesAsync()
//    {
//        try
//        {
//            return await _context.SaveChangesAsync();
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error saving changes");
//            throw;
//        }
//    }

//    public void Attach<T>(T entity) where T : class
//    {
//        if (entity == null) return;
//        _context.Attach(entity);
//    }

//    public void Remove<T>(T entity) where T : class
//    {
//        if (entity == null) return;
//        _context.Remove(entity);
//    }

//    public async Task BeginTransactionAsync()
//    {
//        if (_transaction == null)
//        {
//            _transaction = await _context.Database.BeginTransactionAsync();
//        }
//    }

//    public async Task CommitTransactionAsync()
//    {
//        try
//        {
//            if (_transaction != null)
//            {
//                await _transaction.CommitAsync();
//            }
//        }
//        catch
//        {
//            await RollbackTransactionAsync();
//            throw;
//        }
//        finally
//        {
//            _transaction?.Dispose();
//            _transaction = null;
//        }
//    }

//    public async Task RollbackTransactionAsync()
//    {
//        try
//        {
//            if (_transaction != null)
//            {
//                await _transaction.RollbackAsync();
//            }
//        }
//        finally
//        {
//            _transaction?.Dispose();
//            _transaction = null;
//        }
//    }

//    public void Dispose()
//    {
//        _transaction?.Dispose();
//        _context.Dispose();
//    }
//}
