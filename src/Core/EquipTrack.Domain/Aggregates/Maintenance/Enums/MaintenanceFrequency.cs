namespace EquipTrack.Domain.Aggregates.Maintenance.Enums;

/// <summary>
/// Represents the frequency of preventive maintenance.
/// </summary>
public enum MaintenanceFrequency
{
    Daily = 1,
    Weekly = 2,
    Biweekly = 3,
    Monthly = 4,
    Quarterly = 5,
    SemiAnnually = 6,
    Annually = 7,
    Custom = 99
}