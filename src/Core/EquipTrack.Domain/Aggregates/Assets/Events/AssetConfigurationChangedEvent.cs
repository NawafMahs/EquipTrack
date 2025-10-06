using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Common;

namespace EquipTrack.Domain.Events;

/// <summary>
/// Domain event raised when an asset's configuration changes
/// </summary>
public sealed class AssetConfigurationChangedEvent : BaseEvent
{
    public Guid AssetId { get; }
    public string PropertyName { get; }
    public string? OldValue { get; }
    public string? NewValue { get; }
    public DateTime OccurredOn { get; }

    public AssetConfigurationChangedEvent(
        Guid assetId, 
        string propertyName, 
        string? oldValue, 
        string? newValue)
    {
        AssetId = assetId;
        PropertyName = propertyName;
        OldValue = oldValue;
        NewValue = newValue;
        OccurredOn = DateTime.UtcNow;
    }
}