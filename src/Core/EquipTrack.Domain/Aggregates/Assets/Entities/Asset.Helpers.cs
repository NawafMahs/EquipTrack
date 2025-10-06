using EquipTrack.Domain.Enums;

namespace EquipTrack.Domain.Entities;

/// <summary>
/// Asset partial class - Helper Methods
/// Contains private helper methods and validation logic
/// </summary>
public abstract partial class Asset
{
    #region Private Helpers

    /// <summary>
    /// Validates status transitions based on current state
    /// </summary>
    protected virtual bool IsValidStatusTransition(AssetStatus newStatus)
    {
        return Status switch
        {
            AssetStatus.Active => newStatus is AssetStatus.Running or AssetStatus.Idle or AssetStatus.UnderMaintenance or AssetStatus.OutOfService or AssetStatus.Inactive,
            AssetStatus.Idle => newStatus is AssetStatus.Running or AssetStatus.Active or AssetStatus.UnderMaintenance or AssetStatus.OutOfService,
            AssetStatus.Running => newStatus is AssetStatus.Idle or AssetStatus.Active or AssetStatus.Error or AssetStatus.UnderMaintenance,
            AssetStatus.UnderMaintenance => newStatus is AssetStatus.Active or AssetStatus.Idle or AssetStatus.OutOfService,
            AssetStatus.Error => newStatus is AssetStatus.UnderMaintenance or AssetStatus.OutOfService,
            AssetStatus.OutOfService => newStatus is AssetStatus.UnderMaintenance or AssetStatus.Disposed,
            AssetStatus.Charging => newStatus is AssetStatus.Idle or AssetStatus.Running or AssetStatus.Active,
            AssetStatus.Inactive => newStatus is AssetStatus.Active or AssetStatus.Disposed,
            AssetStatus.Disposed => false,
            _ => false
        };
    }

    #endregion
}