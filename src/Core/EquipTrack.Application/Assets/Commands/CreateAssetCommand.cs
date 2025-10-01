using MediatR;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Enums;
using EquipTrack.Domain.Repositories;

namespace EquipTrack.Application.Assets.Commands;

/// <summary>
/// Command to create a new asset.
/// </summary>
public sealed record CreateAssetCommand : IRequest<Result<Guid>>
{
    /// <summary>
    /// Initializes a new instance of <see cref="CreateAssetCommand"/>.
    /// </summary>
    /// <param name="name">Asset name.</param>
    /// <param name="description">Asset description.</param>
    /// <param name="assetTag">Asset tag or identifier.</param>
    /// <param name="serialNumber">Asset serial number.</param>
    /// <param name="manufacturer">Asset manufacturer.</param>
    /// <param name="model">Asset model.</param>
    /// <param name="location">Asset location.</param>
    /// <param name="criticality">Asset criticality level.</param>
    /// <param name="purchaseDate">Asset purchase date.</param>
    /// <param name="purchaseCost">Asset purchase cost.</param>
    /// <param name="installationDate">Asset installation date.</param>
    /// <param name="warrantyExpirationDate">Asset warranty expiration date.</param>
    public CreateAssetCommand(
        string name,
        string description,
        string assetTag,
        string serialNumber,
        string manufacturer,
        string model,
        string location,
        AssetCriticality criticality,
        DateTime? purchaseDate = null,
        decimal? purchaseCost = null,
        DateTime? installationDate = null,
        DateTime? warrantyExpirationDate = null)
    {
        Name = name;
        Description = description;
        AssetTag = assetTag;
        SerialNumber = serialNumber;
        Manufacturer = manufacturer;
        Model = model;
        Location = location;
        Criticality = criticality;
        PurchaseDate = purchaseDate;
        PurchaseCost = purchaseCost;
        InstallationDate = installationDate;
        WarrantyExpirationDate = warrantyExpirationDate;
    }

    /// <summary>
    /// Asset name.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Asset description.
    /// </summary>
    public string Description { get; init; }

    /// <summary>
    /// Asset tag or identifier.
    /// </summary>
    public string AssetTag { get; init; }

    /// <summary>
    /// Asset serial number.
    /// </summary>
    public string SerialNumber { get; init; }

    /// <summary>
    /// Asset manufacturer.
    /// </summary>
    public string Manufacturer { get; init; }

    /// <summary>
    /// Asset model.
    /// </summary>
    public string Model { get; init; }

    /// <summary>
    /// Asset location.
    /// </summary>
    public string Location { get; init; }

    /// <summary>
    /// Asset criticality level.
    /// </summary>
    public AssetCriticality Criticality { get; init; }

    /// <summary>
    /// Asset purchase date.
    /// </summary>
    public DateTime? PurchaseDate { get; init; }

    /// <summary>
    /// Asset purchase cost.
    /// </summary>
    public decimal? PurchaseCost { get; init; }

    /// <summary>
    /// Asset installation date.
    /// </summary>
    public DateTime? InstallationDate { get; init; }

    /// <summary>
    /// Asset warranty expiration date.
    /// </summary>
    public DateTime? WarrantyExpirationDate { get; init; }
}