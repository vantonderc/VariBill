using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VariBillWebAPI.Data.Context;
using VariBillWebAPI.Data.Repository.Interfaces;

namespace VariBillWebAPI.Data.Repository;

/// <summary>
/// Generic repository implementation.
/// </summary>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly VeriBillTestDBContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="Repository{T}"/> class.
    /// </summary>
    public Repository(VeriBillTestDBContext context, ILogger logger)
    {
        _context = context;
        _dbSet = context.Set<T>();
        _logger = logger;
    }

    public virtual async Task<T?> GetByIdAsync(object id)
        => await _dbSet.FindAsync(id);

    public virtual async Task<IReadOnlyList<T>> GetAllAsync()
        => await _dbSet.ToListAsync();

    public virtual async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.Where(predicate).ToListAsync();

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await Task.CompletedTask;
    }

    public virtual IQueryable<T> GetQueryable()
        => _dbSet.AsQueryable();

    public virtual async Task<T?> FindAsync(params object[] keyValues)
        => await _dbSet.FindAsync(keyValues);
}




