
using System.Linq.Expressions;

namespace VariBillWebAPI.Data.Repository.Interfaces;

/// <summary>
/// Generic repository interface.
/// </summary>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Gets an entity by its primary key.
    /// </summary>
    Task<T?> GetByIdAsync(object id);

    /// <summary>
    /// Gets all entities as a read-only list.
    /// </summary>
    Task<IReadOnlyList<T>> GetAllAsync();

    /// <summary>
    /// Finds entities matching the provided predicate.
    /// </summary>
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Adds a new entity to the set.
    /// </summary>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    Task DeleteAsync(T entity);

    /// <summary>
    /// Returns a queryable for advanced queries.
    /// </summary>
    IQueryable<T> GetQueryable();

    /// <summary>
    /// Finds an entity by composite key values.
    /// </summary>
    Task<T?> FindAsync(params object[] keyValues);
}

