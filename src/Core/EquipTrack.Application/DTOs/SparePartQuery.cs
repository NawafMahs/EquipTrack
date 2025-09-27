namespace EquipTrack.Application.DTOs;

public class SparePartQuery
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int QuantityInStock { get; set; }
    public int MinimumStock { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsLowStock { get; set; }
}

public class CreateSparePartCommand
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int QuantityInStock { get; set; }
    public int MinimumStock { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Notes { get; set; }
}

public class UpdateSparePartCommand
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int QuantityInStock { get; set; }
    public int MinimumStock { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Notes { get; set; }
}

public class UpdateStockCommand
{
    public int Quantity { get; set; }
    public string? Notes { get; set; }
}