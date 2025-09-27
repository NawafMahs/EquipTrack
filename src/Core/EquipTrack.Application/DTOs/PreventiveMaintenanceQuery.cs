using EquipTrack.Domain.Enums;

using EquipTrack.Application.DTOs;
namespace EquipTrack.Application.DTOs;

public class PreventiveMaintenanceQuery
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public MaintenanceFrequency Frequency { get; set; }
    public int FrequencyValue { get; set; }
    public DateTime NextDueDate { get; set; }
    public DateTime? LastCompletedDate { get; set; }
    public bool IsActive { get; set; }
    public decimal EstimatedHours { get; set; }
    public decimal EstimatedCost { get; set; }
    public string? Instructions { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsOverdue { get; set; }
    public bool IsDueSoon { get; set; }

    // Related entities
    public AssetQuery Asset { get; set; } = null!;
    public UserQuery? AssignedToUser { get; set; }
}

public class CreatePreventiveMaintenanceCommand
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public MaintenanceFrequency Frequency { get; set; }
    public int FrequencyValue { get; set; }
    public DateTime NextDueDate { get; set; }
    public decimal EstimatedHours { get; set; }
    public decimal EstimatedCost { get; set; }
    public string? Instructions { get; set; }
    public Guid AssetId { get; set; }
    public Guid? AssignedToUserId { get; set; }
}

public class UpdatePreventiveMaintenanceCommand
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public MaintenanceFrequency Frequency { get; set; }
    public int FrequencyValue { get; set; }
    public DateTime NextDueDate { get; set; }
    public bool IsActive { get; set; }
    public decimal EstimatedHours { get; set; }
    public decimal EstimatedCost { get; set; }
    public string? Instructions { get; set; }
    public Guid? AssignedToUserId { get; set; }
}