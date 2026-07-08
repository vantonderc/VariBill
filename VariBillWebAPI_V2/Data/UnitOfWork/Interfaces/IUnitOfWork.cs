using VariBillWebAPI.Data.Repository.Interfaces;

namespace VariBillWebAPI.Data.UnitOfWork.Interfaces;

/// <summary>
/// Unit of work interface.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Repository for product entities.
    /// </summary>
    IProductRepository Products { get; }

    /// <summary>
    /// Repository for product type entities.
    /// </summary>
    IProductTypeRepository ProductTypes { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}



