namespace EquipTrack.Core.SharedKernel;

/// <summary>
/// Represents an entity with a unique identifier.
/// </summary>
/// <typeparam name="TKey">The type of the entity's primary key.</typeparam>
public interface IEntity<out TKey>
{
    /// <summary>
    /// Gets the unique identifier of the entity.
    /// </summary>
    TKey Id { get; }
}