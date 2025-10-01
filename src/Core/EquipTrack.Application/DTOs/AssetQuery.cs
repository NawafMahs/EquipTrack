
using EquipTrack.Application.DTOs;
using EquipTrack.Domain.Enums;
using EquipTrack.Domain.Repositories;
namespace EquipTrack.Application.DTOs;

/// <summary>
/// Query result for Asset entity.
/// </summary>
public class AssetQuery
{
    /// <summary>
    /// Asset unique identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Asset name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Asset description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Asset tag or identifier.
    /// </summary>
    public string AssetTag { get; set; } = string.Empty;

    /// <summary>
    /// Asset serial number.
    /// </summary>
    public string SerialNumber { get; set; } = string.Empty;

    /// <summary>
    /// Asset manufacturer.
    /// </summary>
    public string Manufacturer { get; set; } = string.Empty;

    /// <summary>
    /// Asset model.
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Asset location.
    /// </summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Asset status.
    /// </summary>
    public AssetStatus Status { get; set; }

    /// <summary>
    /// Asset criticality level.
    /// </summary>
    public AssetCriticality Criticality { get; set; }

    /// <summary>
    /// Asset purchase date.
    /// </summary>
    public DateTime? PurchaseDate { get; set; }

    /// <summary>
    /// Asset purchase cost.
    /// </summary>
    public decimal? PurchaseCost { get; set; }

    /// <summary>
    /// Asset installation date.
    /// </summary>
    public DateTime? InstallationDate { get; set; }

    /// <summary>
    /// Asset warranty expiration date.
    /// </summary>
    public DateTime? WarrantyExpirationDate { get; set; }

    /// <summary>
    /// Indicates if the asset is under warranty.
    /// </summary>
    public bool IsUnderWarranty { get; set; }

    /// <summary>
    /// Indicates if the asset is operational.
    /// </summary>
    public bool IsOperational { get; set; }

    /// <summary>
    /// Date when the asset was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date when the asset was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Number of active work orders for this asset.
    /// </summary>
    public int ActiveWorkOrdersCount { get; set; }

    /// <summary>
    /// Total number of work orders for this asset.
    /// </summary>
    public int TotalWorkOrdersCount { get; set; }
}