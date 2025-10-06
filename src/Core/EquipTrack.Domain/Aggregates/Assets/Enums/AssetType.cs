namespace EquipTrack.Domain.Enums;

/// <summary>
/// Discriminator enum for Asset Table-Per-Hierarchy (TPH) inheritance.
/// Defines the type of asset in the CMMS system.
/// </summary>
public enum AssetType
{
    /// <summary>
    /// Generic asset type (default)
    /// </summary>
    Generic = 0,

    /// <summary>
    /// Industrial machine or equipment
    /// </summary>
    Machine = 1,

    /// <summary>
    /// Autonomous or industrial robot
    /// </summary>
    Robot = 2,

    /// <summary>
    /// Sensor or monitoring device
    /// </summary>
    Sensor = 3,

    /// <summary>
    /// Vehicle or mobile equipment
    /// </summary>
    Vehicle = 4,

    /// <summary>
    /// Conveyor system
    /// </summary>
    Conveyor = 5,

    /// <summary>
    /// HVAC system
    /// </summary>
    HVAC = 6,

    /// <summary>
    /// Pump equipment
    /// </summary>
    Pump = 7,

    /// <summary>
    /// Compressor equipment
    /// </summary>
    Compressor = 8,

    /// <summary>
    /// Generator equipment
    /// </summary>
    Generator = 9,

    /// <summary>
    /// Tool or hand-held equipment
    /// </summary>
    Tool = 10
}