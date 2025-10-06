namespace EquipTrack.Core.SharedKernel;

/// <summary>
/// Represents the base entity with a unique identifier of type <see cref="Guid"/>.
/// Provides automatic ID generation upon creation unless explicitly set.
/// </summary>
public abstract class BaseEntity : IEntity<Guid>
{
    private readonly List<BaseEvent> _domainEvents = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEntity"/> class
    /// with a new unique identifier generated automatically.
    /// </summary>
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEntity"/> class
    /// with the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier to assign to this entity.</param>
    protected BaseEntity(Guid id)
    {
        Id = id;
        CreatedAt = DateTime.UtcNow;
    }

    public IEnumerable<BaseEvent> DoaminEvents => _domainEvents.AsReadOnly();


    /// <summary>
    /// Gets the unique identifier of the entity.
    /// This value is assigned at creation and cannot be modified afterwards.
    /// </summary>
    public Guid Id { get; private init; }

    /// <summary>
    /// Gets the date and time when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>
    /// Gets the date and time when the entity was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; protected set; }

    /// <summary>
    /// Updates the UpdatedAt timestamp to the current UTC time.
    /// </summary>
    protected void MarkAsUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    protected void AddDomainEvent(BaseEvent eventItem) => _domainEvents.Add(eventItem);

    public void ClearDomainEvents() => _domainEvents.Clear();

}