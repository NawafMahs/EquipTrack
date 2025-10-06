using EquipTrack.Domain.Common;
using EquipTrack.Domain.Enums;
using EquipTrack.Domain.Events;

namespace EquipTrack.Domain.Entities;

/// <summary>
/// Asset partial class - Core Behaviors
/// Contains methods for updating core asset properties
/// </summary>
public abstract partial class Asset
{
    #region Core Behaviors

    /// <summary>
    /// Updates the asset's name
    /// </summary>
    public void UpdateName(string name)
    {
        var oldName = Name;
        Name = Ensure.NotEmpty(name, nameof(name));
        
        AddDomainEvent(new AssetConfigurationChangedEvent(Id, Name, oldName, name));
        AddLog(AssetLogType.ConfigurationChanged, $"Name updated to: {name}", LogSeverity.Info);
    }

    /// <summary>
    /// Updates the asset's description
    /// </summary>
    public void UpdateDescription(string description)
    {
        Description = Ensure.NotEmpty(description, nameof(description));
    }

    /// <summary>
    /// Sets the asset's name (alias for UpdateName for consistency)
    /// </summary>
    public void SetName(string name) => UpdateName(name);

    /// <summary>
    /// Sets the asset's description (alias for UpdateDescription for consistency)
    /// </summary>
    public void SetDescription(string description) => UpdateDescription(description);

    /// <summary>
    /// Sets the asset tag
    /// </summary>
    public void SetAssetTag(string assetTag)
    {
        var oldTag = AssetTag;
        AssetTag = Ensure.NotEmpty(assetTag, nameof(assetTag));
        
        AddDomainEvent(new AssetConfigurationChangedEvent(Id, nameof(AssetTag), oldTag, assetTag));
        AddLog(AssetLogType.ConfigurationChanged, $"Asset tag updated to: {assetTag}", LogSeverity.Info);
    }

    /// <summary>
    /// Sets the serial number
    /// </summary>
    public void SetSerialNumber(string serialNumber)
    {
        var oldSerial = SerialNumber;
        SerialNumber = Ensure.NotEmpty(serialNumber, nameof(serialNumber));
        
        AddDomainEvent(new AssetConfigurationChangedEvent(Id, nameof(SerialNumber), oldSerial, serialNumber));
        AddLog(AssetLogType.ConfigurationChanged, $"Serial number updated to: {serialNumber}", LogSeverity.Info);
    }

    /// <summary>
    /// Sets the manufacturer
    /// </summary>
    public void SetManufacturer(string manufacturer)
    {
        Manufacturer = Ensure.NotEmpty(manufacturer, nameof(manufacturer));
    }

    /// <summary>
    /// Sets the model
    /// </summary>
    public void SetModel(string model)
    {
        Model = Ensure.NotEmpty(model, nameof(model));
    }

    /// <summary>
    /// Updates the asset's location
    /// </summary>
    public virtual void UpdateLocation(string newLocation)
    {
        var oldLocation = Location;
        Location = Ensure.NotEmpty(newLocation, nameof(newLocation));
        
        AddDomainEvent(new AssetLocationChangedEvent(Id, oldLocation, newLocation));
        AddLog(AssetLogType.ConfigurationChanged, $"Location changed from '{oldLocation}' to '{newLocation}'", LogSeverity.Info);
    }

    /// <summary>
    /// Changes the asset's operational status with validation
    /// </summary>
    public virtual void ChangeStatus(AssetStatus newStatus)
    {
        if (!IsValidStatusTransition(newStatus))
            throw new InvalidOperationException($"Invalid status transition: {Status} → {newStatus}");

        var oldStatus = Status;
        Status = newStatus;
        
        AddDomainEvent(new AssetStatusChangedEvent(Id, oldStatus, newStatus));
        AddLog(AssetLogType.StatusChange, $"Status changed from {oldStatus} to {newStatus}", LogSeverity.Info);
    }

    /// <summary>
    /// Updates the asset's status (alias for ChangeStatus for consistency)
    /// </summary>
    public void UpdateStatus(AssetStatus newStatus) => ChangeStatus(newStatus);

    /// <summary>
    /// Updates the asset's criticality level
    /// </summary>
    public void UpdateCriticality(AssetCriticality criticality)
    {
        var oldCriticality = Criticality;
        Criticality = criticality;
        
        AddDomainEvent(new AssetCriticalityChangedEvent(Id, oldCriticality, criticality));
        AddLog(AssetLogType.ConfigurationChanged, $"Criticality updated to: {criticality}", LogSeverity.Info);
    }

    /// <summary>
    /// Sets the asset's criticality (alias for UpdateCriticality for consistency)
    /// </summary>
    public void SetCriticality(AssetCriticality criticality) => UpdateCriticality(criticality);

    /// <summary>
    /// Sets the installation date
    /// </summary>
    public void SetInstallationDate(DateTime? installationDate)
    {
        if (installationDate.HasValue)
        {
            if (installationDate.Value < PurchaseDate)
                throw new ArgumentException("Installation date cannot be before purchase date.", nameof(installationDate));

            InstallationDate = installationDate.Value;
        }
    }

    /// <summary>
    /// Sets the warranty expiry date
    /// </summary>
    public void SetWarrantyExpiry(DateTime expiryDate)
    {
        if (expiryDate < PurchaseDate)
            throw new ArgumentException("Warranty expiry cannot be before purchase date.", nameof(expiryDate));

        WarrantyExpiryDate = expiryDate;
    }

    /// <summary>
    /// Sets the warranty expiration date (alias for SetWarrantyExpiry for consistency)
    /// </summary>
    public void SetWarrantyExpirationDate(DateTime? expiryDate)
    {
        if (expiryDate.HasValue)
            SetWarrantyExpiry(expiryDate.Value);
    }

    /// <summary>
    /// Updates purchase information
    /// </summary>
    public void UpdatePurchaseInfo(DateTime? purchaseDate, decimal? purchasePrice)
    {
        if (purchaseDate.HasValue)
            PurchaseDate = purchaseDate.Value;
        
        if (purchasePrice.HasValue)
            PurchasePrice = Ensure.Positive(purchasePrice.Value, nameof(purchasePrice));
    }

    /// <summary>
    /// Sets purchase information (alias for UpdatePurchaseInfo for consistency)
    /// </summary>
    public void SetPurchaseInfo(DateTime? purchaseDate, decimal? purchasePrice) => UpdatePurchaseInfo(purchaseDate, purchasePrice);

    /// <summary>
    /// Sets the asset's image URL
    /// </summary>
    public void SetImageUrl(string imageUrl)
    {
        ImageUrl = Ensure.NotEmpty(imageUrl, nameof(imageUrl));
    }

    /// <summary>
    /// Adds a note to the asset
    /// </summary>
    public void AddNote(string note)
    {
        note = Ensure.NotEmpty(note, nameof(note));
        Notes = string.IsNullOrWhiteSpace(Notes) 
            ? note 
            : $"{Notes}\n{DateTime.UtcNow:yyyy-MM-dd HH:mm}: {note}";
        
        AddDomainEvent(new AssetNoteAddedEvent(Id, note));
        AddLog(AssetLogType.ManualEntry, $"Note added: {note}", LogSeverity.Info);
    }

    /// <summary>
    /// Adds or updates metadata
    /// </summary>
    public void SetMetadata(string key, string value)
    {
        Ensure.NotEmpty(key, nameof(key));
        Metadata[key] = value;
    }

    /// <summary>
    /// Gets metadata value by key
    /// </summary>
    public string? GetMetadata(string key)
    {
        return Metadata.TryGetValue(key, out var value) ? value : null;
    }

    #endregion
}