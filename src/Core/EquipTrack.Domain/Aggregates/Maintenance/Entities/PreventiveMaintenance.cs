using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Domain.Entities;

public class PreventiveMaintenance : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public MaintenanceFrequency Frequency { get; set; }
    public int FrequencyValue { get; set; } // e.g., 30 for every 30 days
    public DateTime NextDueDate { get; set; }
    public DateTime? LastCompletedDate { get; set; }
    public bool IsActive { get; set; } = true;
    public decimal EstimatedHours { get; set; }
    public decimal EstimatedCost { get; set; }
    public string? Instructions { get; set; }

    // Foreign Keys
    public Guid AssetRef { get; set; }
    public Guid? AssignedToUserRef { get; set; }

    // Navigation properties
    public virtual Asset Asset { get; set; } = null!;
    public virtual User? AssignedToUser { get; set; }

    public bool IsOverdue => NextDueDate < DateTime.UtcNow;
    public bool IsDueSoon => NextDueDate <= DateTime.UtcNow.AddDays(7);
}