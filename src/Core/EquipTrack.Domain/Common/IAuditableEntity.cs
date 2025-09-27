namespace EquipTrack.Domain.Common;

public interface IAuditableEntity
{
    DateTime CreatedAt { get; }
    DateTime? UpdatedAt { get; }
    string? CreatedBy { get; }
    string? UpdatedBy { get; }
}
