namespace EquipTrack.Domain.Enums;

/// <summary>
/// Represents the operational status of a robot.
/// </summary>
public enum RobotStatus
{
    Idle = 0,
    Running = 1,
    Charging = 2,
    Error = 3,
    Maintenance = 4,
    OutOfService = 5
}
