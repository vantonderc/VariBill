using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
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

//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;
//using VariBillWebAPI.Data.Context;
//using VariBillWebAPI.Data.Repository.Interface;

//namespace VariBillWebAPI.Data.Repository;

////TODO(NB)->ensure no usings above refer to bongoe related code
//public class Repository<T> : IRepository<T> where T : class
//{
//    protected readonly VeriBillTestDBContext _context;
//    protected readonly DbSet<T> _dbSet;
//    protected readonly ILogger _logger;

//    public Repository(VeriBillTestDBContext context, ILogger logger)
//    {
//        _context = context;
//        _dbSet = context.Set<T>();
//        _logger = logger;
//    }

//    public virtual async Task<T> GetByIdAsync(string id)
//    {
//        return await _dbSet.FindAsync(id);
//    }

//    public virtual async Task<IEnumerable<T>> GetAllAsync()
//    {
//        return await _dbSet.ToListAsync();
//    }

//    public virtual async Task<T> AddAsync(T entity)
//    {
//        await _dbSet.AddAsync(entity);
//        return entity;
//    }

//    public virtual async Task UpdateAsync(T entity)
//    {
//        _dbSet.Update(entity);
//        await Task.CompletedTask;
//    }

//    public virtual void Update(T entity)
//    {
//        _dbSet.Update(entity);
//    }

//    public virtual async Task DeleteAsync(T entity)
//    {
//        _dbSet.Remove(entity);
//        await Task.CompletedTask;
//    }

//    public virtual void Remove(T entity)
//    {
//        _dbSet.Remove(entity);
//    }

//    public virtual IQueryable<T> GetQueryable()
//    {
//        return _dbSet.AsQueryable();
//    }

//    public virtual async Task<T> FindAsync(params object[] keyValues)
//    {
//        return await _dbSet.FindAsync(keyValues);
//    }

//    public virtual async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
//    {
//        return await _dbSet.Where(predicate).ToListAsync();
//    }
//}
