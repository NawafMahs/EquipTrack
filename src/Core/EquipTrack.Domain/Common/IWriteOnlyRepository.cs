using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Domain.Common;

/// <summary>
/// Represents a write-only repository for a specific entity type.
/// Provides methods to add, update, and remove entities.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
/// <typeparam name="TKey">The type of the entity key.</typeparam>
public interface IWriteOnlyRepository<TEntity, TKey> : IDisposable
    where TEntity : IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    #region Query Methods (for update operations)

    /// <summary>
    /// Gets an entity by its key. This is needed for update operations.
    /// </summary>
    /// <param name="id">The key of the entity to retrieve.</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    Task<TEntity?> GetByIdAsync(TKey id);

    #endregion

    #region Synchronous Methods

    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    void Add(TEntity entity);

    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(TEntity entity);

    /// <summary>
    /// Removes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    void Remove(TEntity entity);

    /// <summary>
    /// Removes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    void Delete(TEntity entity);

    /// <summary>
    /// Removes an entity by its key.
    /// </summary>
    /// <param name="id">The key of the entity to remove.</param>
    void Remove(TKey id);

    /// <summary>
    /// Adds multiple entities to the repository in a batch operation.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    void AddRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Removes multiple entities from the repository in a batch operation.
    /// </summary>
    /// <param name="entities">The entities to remove.</param>
    void RemoveRange(IEnumerable<TEntity> entities);

    #endregion

    #region Asynchronous Methods

    /// <summary>
    /// Asynchronously adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    Task AddAsync(TEntity entity);

    /// <summary>
    /// Asynchronously removes an entity by its key.
    /// </summary>
    /// <param name="id">The key of the entity to remove.</param>
    Task RemoveByIdAsync(TKey id);

    /// <summary>
    /// Asynchronously adds multiple entities to the repository in a batch operation.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    Task AddRangeAsync(IEnumerable<TEntity> entities);

    #endregion
}