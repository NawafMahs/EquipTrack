namespace EquipTrack.Domain.Aggregates.SpareParts.Enums;

/// <summary>
/// Represents the category of spare parts in the CMMS system.
/// </summary>
public enum SparePartCategory
{
    Mechanical = 1,
    Electrical = 2,
    Electronic = 3,
    Hydraulic = 4,
    Pneumatic = 5,
    Consumable = 6,
    SafetyEquipment = 7,
    Tools = 8,
    Other = 99
}