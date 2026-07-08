using Microsoft.EntityFrameworkCore.Storage;
using VariBillWebAPI.Data.Context;
using VariBillWebAPI.Data.Repository.Interfaces;
using VariBillWebAPI.Data.UnitOfWork.Interfaces;

namespace VariBillWebAPI.Data.UnitOfWork;

/// <summary>
/// Unit of work implementation.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly VeriBillTestDBContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(
        VeriBillTestDBContext context,
        ILogger<UnitOfWork> logger,
        IProductRepository productRepository,
        IProductTypeRepository productTypeRepository)
    {
        _context = context;
        _logger = logger;
        Products = productRepository;
        ProductTypes = productTypeRepository;
    }

    public IProductRepository Products { get; }
    public IProductTypeRepository ProductTypes { get; }

    public async Task<int> SaveChangesAsync()
    {
        try
        {
            return await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes.");
            throw;
        }
    }

    public async Task BeginTransactionAsync()
    {
        _transaction ??= await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            if (_transaction != null)
                await _transaction.CommitAsync();
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        try
        {
            if (_transaction != null)
                await _transaction.RollbackAsync();
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}




