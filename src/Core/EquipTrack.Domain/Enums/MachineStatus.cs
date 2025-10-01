namespace EquipTrack.Domain.Enums;

/// <summary>
/// Represents the operational status of a machine.
/// </summary>
public enum MachineStatus
{
    Idle = 0,
    Running = 1,
    Error = 2,
    Maintenance = 3,
    OutOfService = 4
}
