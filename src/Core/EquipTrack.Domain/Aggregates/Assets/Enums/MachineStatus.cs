namespace EquipTrack.Domain.Enums;

/// <summary>
/// Represents the operational status of a machine.
/// </summary>
[Obsolete("Use AssetStatus instead. This enum is deprecated and will be removed in a future version.")]
public enum MachineStatus
{
    Idle = 0,
    Running = 1,
    Error = 2,
    Maintenance = 3,
    OutOfService = 4
}
