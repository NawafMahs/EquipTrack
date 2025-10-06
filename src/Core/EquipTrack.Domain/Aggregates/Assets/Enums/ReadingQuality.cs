namespace EquipTrack.Domain.Enums;

/// <summary>
/// Quality indicator for sensor readings.
/// Indicates the reliability of a sensor measurement.
/// </summary>
public enum ReadingQuality
{
    /// <summary>
    /// Good quality reading
    /// </summary>
    Good = 0,

    /// <summary>
    /// Uncertain quality reading
    /// </summary>
    Uncertain = 1,

    /// <summary>
    /// Bad quality reading
    /// </summary>
    Bad = 2,

    /// <summary>
    /// Reading is out of range
    /// </summary>
    OutOfRange = 3,

    /// <summary>
    /// Sensor needs calibration
    /// </summary>
    NeedsCalibration = 4
}