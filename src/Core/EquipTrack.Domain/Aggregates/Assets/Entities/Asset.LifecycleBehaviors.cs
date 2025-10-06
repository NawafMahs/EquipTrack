using EquipTrack.Domain.Enums;
using EquipTrack.Domain.Events;

namespace EquipTrack.Domain.Entities;

/// <summary>
/// Asset partial class - Lifecycle Behaviors
/// Contains methods for asset lifecycle management (activation, deactivation, disposal)
/// </summary>
public abstract partial class Asset
{
    #region Lifecycle Behaviors

    /// <summary>
    /// Activates the asset
    /// </summary>
    public virtual void Activate()
    {
        IsActive = true;
        Status = AssetStatus.Active;
        
        AddDomainEvent(new AssetActivatedEvent(Id));
        AddLog(AssetLogType.StatusChange, "Asset activated", LogSeverity.Info);
    }

    /// <summary>
    /// Deactivates the asset
    /// </summary>
    public virtual void Deactivate()
    {
        IsActive = false;
        Status = AssetStatus.Inactive;
        
        AddDomainEvent(new AssetDeactivatedEvent(Id));
        AddLog(AssetLogType.StatusChange, "Asset deactivated", LogSeverity.Warning);
    }

    /// <summary>
    /// Marks asset as disposed
    /// </summary>
    public void Dispose(string reason)
    {
        IsActive = false;
        Status = AssetStatus.Disposed;
        
        AddNote($"Asset disposed. Reason: {reason}");
        AddDomainEvent(new AssetDisposedEvent(Id, reason));
        AddLog(AssetLogType.StatusChange, $"Asset disposed: {reason}", LogSeverity.Warning);
    }

    #endregion
}