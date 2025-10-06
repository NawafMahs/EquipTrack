namespace EquipTrack.Domain.Aggregates.SpareParts.Enums;

/// <summary>
/// Represents the status of spare part stock levels.
/// </summary>
public enum StockStatus
{
    InStock = 1,
    LowStock = 2,
    OutOfStock = 3,
    OnOrder = 4,
    Discontinued = 5
}