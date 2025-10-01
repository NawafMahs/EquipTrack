using EquipTrack.Domain.Common;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Domain.Entities;

/// <summary>
/// Represents a physical machine asset.
/// Specialized type of Asset with machine-specific properties.
/// </summary>
public class Machine : Asset
{
    #region Machine-Specific Properties

    /// <summary>
    /// Power rating in kilowatts
    /// </summary>
    public decimal? PowerRating { get; private set; }

    /// <summary>
    /// Voltage requirement (e.g., "220V", "380V 3-phase")
    /// </summary>
    public string? VoltageRequirement { get; private set; }

    /// <summary>
    /// Reference to machine type/category
    /// </summary>
    public string? MachineTypeRef { get; private set; }

    /// <summary>
    /// Current operational efficiency percentage (0-100)
    /// </summary>
    public decimal? CurrentEfficiency { get; private set; }

    /// <summary>
    /// Maximum operating temperature in Celsius
    /// </summary>
    public decimal? MaxOperatingTemperature { get; private set; }

    /// <summary>
    /// Duty cycle percentage
    /// </summary>
    public decimal? DutyCycle { get; private set; }

    #endregion

    #region Constructors

    /// <summary>
    /// EF Core parameterless constructor
    /// </summary>
    protected Machine() : base() { }

    /// <summary>
    /// Private constructor for factory method
    /// </summary>
    private Machine(
        string name,
        string description,
        string serialNumber,
        string model,
        string manufacturer,
        string assetTag,
        string location,
        DateTime purchaseDate,
        decimal purchasePrice,
        AssetCriticality criticality,
        string? machineTypeRef = null,
        decimal? powerRating = null,
        string? voltageRequirement = null)
        : base(name, description, serialNumber, model, manufacturer, assetTag, AssetType.Machine, location, purchaseDate, purchasePrice, criticality)
    {
        MachineTypeRef = machineTypeRef;
        PowerRating = powerRating;
        VoltageRequirement = voltageRequirement;
    }

    #endregion

    #region Factory Methods

    /// <summary>
    /// Creates a new machine instance
    /// </summary>
    public static Machine Create(
        string name,
        string description,
        string serialNumber,
        string model,
        string manufacturer,
        string assetTag,
        string location,
        DateTime purchaseDate,
        decimal purchasePrice,
        AssetCriticality criticality = AssetCriticality.Medium,
        string? machineTypeRef = null,
        decimal? powerRating = null,
        string? voltageRequirement = null)
    {
        var machine = new Machine(
            name, description, serialNumber, model, manufacturer, assetTag,
            location, purchaseDate, purchasePrice, criticality,
            machineTypeRef, powerRating, voltageRequirement);

        machine.AddLog(AssetLogType.StatusChange, $"Machine '{name}' registered in system", LogSeverity.Info);
        return machine;
    }

    #endregion

    #region Machine-Specific Behaviors

    /// <summary>
    /// Updates the machine's efficiency
    /// </summary>
    public void UpdateEfficiency(decimal efficiency)
    {
        if (efficiency < 0 || efficiency > 100)
            throw new ArgumentException("Efficiency must be between 0 and 100.", nameof(efficiency));

        var oldEfficiency = CurrentEfficiency;
        CurrentEfficiency = efficiency;

        var severity = efficiency < 70 ? LogSeverity.Warning : LogSeverity.Info;
        AddLog(AssetLogType.PerformanceAlert, 
            $"Efficiency updated from {oldEfficiency?.ToString() ?? "N/A"}% to {efficiency}%", 
            severity);
    }

    /// <summary>
    /// Sets the power rating
    /// </summary>
    public void SetPowerRating(decimal powerRating, string? voltageRequirement = null)
    {
        if (powerRating < 0)
            throw new ArgumentException("Power rating cannot be negative.", nameof(powerRating));

        PowerRating = powerRating;
        if (!string.IsNullOrWhiteSpace(voltageRequirement))
            VoltageRequirement = voltageRequirement;

        AddLog(AssetLogType.ConfigurationChanged, 
            $"Power rating set to {powerRating}kW" + (voltageRequirement != null ? $" at {voltageRequirement}" : ""), 
            LogSeverity.Info);
    }

    /// <summary>
    /// Sets the machine type reference
    /// </summary>
    public void SetMachineType(string machineTypeRef)
    {
        MachineTypeRef = Ensure.NotEmpty(machineTypeRef, nameof(machineTypeRef));
        AddLog(AssetLogType.ConfigurationChanged, $"Machine type set to: {machineTypeRef}", LogSeverity.Info);
    }

    /// <summary>
    /// Sets the maximum operating temperature
    /// </summary>
    public void SetMaxOperatingTemperature(decimal temperature)
    {
        MaxOperatingTemperature = temperature;
        SetMetadata("MaxOperatingTemperature", temperature.ToString());
    }

    /// <summary>
    /// Sets the duty cycle
    /// </summary>
    public void SetDutyCycle(decimal dutyCycle)
    {
        if (dutyCycle < 0 || dutyCycle > 100)
            throw new ArgumentException("Duty cycle must be between 0 and 100.", nameof(dutyCycle));

        DutyCycle = dutyCycle;
        SetMetadata("DutyCycle", dutyCycle.ToString());
    }

    /// <summary>
    /// Checks if machine is running efficiently
    /// </summary>
    public bool IsRunningEfficiently(decimal threshold = 80)
    {
        return CurrentEfficiency.HasValue && CurrentEfficiency.Value >= threshold;
    }

    #endregion

    #region Overrides

    public override string ToString()
    {
        return $"Machine: {Name} ({Model}) - {SerialNumber}";
    }

    #endregion
}
