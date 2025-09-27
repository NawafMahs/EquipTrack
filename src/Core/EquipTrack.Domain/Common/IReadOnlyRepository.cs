using System.Linq.Expressions;
using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Domain.Common;

/// <summary>
/// Represents a read-only repository for managing entities.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
/// <typeparam name="TKey">The type of the entity's key.</typeparam>
public interface IReadOnlyRepository<TEntity, in TKey> : IDisposable
    where TEntity : IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Gets the number of entities that match the specified predicate.
    /// </summary>
    /// <param name="predicate">A function to test each entity for a condition.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The count of matching entities.</returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities.
    /// </summary>
    /// <returns>A list of all entities.</returns>
    Task<List<TEntity>> GetAllAsync();

    /// <summary>
    /// Gets all entities as a queryable.
    /// </summary>
    /// <returns>A queryable of all entities.</returns>
    IQueryable<TEntity> GetAllQueryable();

    /// <summary>
    /// Gets all entities projected to a specified type.
    /// </summary>
    /// <typeparam name="TProjection">The type to project entities to.</typeparam>
    /// <param name="selector">A function to transform each entity.</param>
    /// <returns>A list of projected entities.</returns>
    IQueryable<TProjection> GetAll<TProjection>(Expression<Func<TEntity, TProjection>> selector);

    /// <summary>
    /// Gets a single entity that matches the specified predicate and projects it to a specified type.
    /// </summary>
    /// <typeparam name="TProjection">The type to project the entity to.</typeparam>
    /// <param name="predicate">A function to test the entity for a condition.</param>
    /// <param name="selector">A function to transform the entity.</param>
    /// <returns>The projected entity if found; otherwise, null.</returns>
    Task<TProjection?> GetAsync<TProjection>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TProjection>> selector);

    /// <summary>
    /// Gets an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    Task<TEntity?> GetByIdAsync(TKey id);

    /// <summary>
    /// Gets a single entity that matches the specified predicate.
    /// </summary>
    /// <param name="predicate">A function to test the entity for a condition.</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Checks if any entity matches the specified predicate.
    /// </summary>
    /// <param name="predicate">A function to test each entity for a condition.</param>
    /// <returns>True if any entity matches; otherwise, false.</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Checks if an entity exists with the specified key.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>True if an entity exists with the specified key; otherwise, false.</returns>
    Task<bool> AnyAsync(TKey id);
}