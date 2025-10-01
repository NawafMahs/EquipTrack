using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Domain.Common;

/// <summary>
/// Base class for all entities in the system.
/// Provides the primary key and audit tracking fields.
/// </summary>
public abstract class BaseEntity : IEntity<Guid>, IAuditableEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Primary identifier for the entity.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// The UTC date and time when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get;private set; } = DateTime.UtcNow;

    /// <summary>
    /// The UTC date and time when the entity was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// The username or identifier of the user who created this entity.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// The username or identifier of the user who last updated this entity.
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Domain events raised by this entity.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    #region Behavior Methods

    /// <summary>
    /// Updates the audit fields when an entity is modified.
    /// </summary>
    /// <param name="updatedBy">User who updated the entity.</param>
    public void SetUpdated(string updatedBy)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Sets the creator info for the entity.
    /// Should only be called when the entity is first created.
    /// </summary>
    /// <param name="createdBy">User who created the entity.</param>
    public void SetCreated(string createdBy)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    /// <summary>
    /// Adds a domain event for later processing.
    /// </summary>
    protected void AddDomainEvent(IDomainEvent @event)
    {
        _domainEvents.Add(@event);
    }

    /// <summary>
    /// Clears all domain events after they are dispatched.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    #endregion
}

