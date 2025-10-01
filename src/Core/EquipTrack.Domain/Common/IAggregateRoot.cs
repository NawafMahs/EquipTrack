namespace EquipTrack.Domain.Common;

/// <summary>
/// Marker interface used to indicate that an entity is the 
/// root of an Aggregate in the Domain-Driven Design (DDD) pattern.
/// 
/// Aggregate Roots are responsible for:
/// - Enforcing invariants of the entire Aggregate.
/// - Controlling access to child entities and value objects.
/// - Acting as the entry point for repository operations.
/// 
/// Note: This interface is intentionally left empty and serves only 
/// as a semantic marker for DDD consistency.
/// </summary>
public interface IAggregateRoot
{
    // No members are required.
    // This interface is used solely as a marker.
}
