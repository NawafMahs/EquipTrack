using EquipTrack.Domain.Events;

namespace EquipTrack.Domain.Entities;

/// <summary>
/// Asset partial class - Maintenance Behaviors
/// Contains methods for maintenance operations and operational metrics
/// </summary>
public abstract partial class Asset
{
    #region Maintenance Behaviors

    /// <summary>
    /// Records maintenance activity
    /// </summary>
    public virtual void RecordMaintenance(DateTime maintenanceDate, DateTime? nextScheduledDate = null, string? notes = null)
    {
        LastMaintenanceDate = maintenanceDate;
        NextMaintenanceDate = nextScheduledDate;

        AddDomainEvent(new AssetMaintenanceRecordedEvent(Id, maintenanceDate, nextScheduledDate, notes));
        
        var message = $"Maintenance performed on {maintenanceDate:yyyy-MM-dd}";
        if (nextScheduledDate.HasValue)
            message += $". Next scheduled: {nextScheduledDate.Value:yyyy-MM-dd}";
        if (!string.IsNullOrWhiteSpace(notes))
            message += $". Notes: {notes}";

        AddLog(Enums.AssetLogType.MaintenancePerformed, message, Enums.LogSeverity.Info);
    }

    /// <summary>
    /// Increments operating hours
    /// </summary>
    public virtual void IncrementOperatingHours(int hours)
    {
        if (hours < 0)
            throw new ArgumentException("Operating hours cannot be negative.", nameof(hours));

        OperatingHours += hours;
    }

    /// <summary>
    /// Checks if asset requires maintenance
    /// </summary>
    public bool RequiresMaintenance()
    {
        return NextMaintenanceDate.HasValue && NextMaintenanceDate.Value <= DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if asset is under warranty
    /// </summary>
    public bool IsUnderWarranty()
    {
        return WarrantyExpiryDate.HasValue && WarrantyExpiryDate.Value > DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if asset is operational
    /// </summary>
    public bool IsOperational()
    {
        return IsActive && (Status == Enums.AssetStatus.Active || Status == Enums.AssetStatus.Running || Status == Enums.AssetStatus.Idle);
    }

    #endregion
}