using EquipTrack.Domain.Common;

namespace EquipTrack.Domain.Entities;

/// <summary>
/// Tracks spare part usage for machines.
/// Links machines to spare parts with usage details.
/// </summary>
public class MachineSparePartUsage : BaseEntity
{
    public Guid MachineRef { get; init; }
    public Guid SparePartRef { get; init; }
    public int QuantityUsed { get; private set; }
    public DateTime UsageDate { get; init; }
    public string? Reason { get; private set; }
    public Guid? WorkOrderRef { get; init; }
    public string? InstalledBy { get; private set; }

    // EF Core constructor
    protected MachineSparePartUsage() { }

    private MachineSparePartUsage(
        Guid machineRef,
        Guid sparePartRef,
        int quantityUsed,
        DateTime usageDate,
        string? reason = null,
        Guid? workOrderRef = null,
        string? installedBy = null)
    {
        MachineRef = machineRef;
        SparePartRef = sparePartRef;
        QuantityUsed = Ensure.Positive(quantityUsed, nameof(quantityUsed));
        UsageDate = usageDate;
        Reason = reason;
        WorkOrderRef = workOrderRef;
        InstalledBy = installedBy;
    }

    public static MachineSparePartUsage Create(
        Guid machineRef,
        Guid sparePartRef,
        int quantityUsed,
        DateTime usageDate,
        string? reason = null,
        Guid? workOrderRef = null,
        string? installedBy = null)
    {
        return new MachineSparePartUsage(machineRef, sparePartRef, quantityUsed, usageDate, reason, workOrderRef, installedBy);
    }

    public void UpdateQuantity(int newQuantity)
    {
        QuantityUsed = Ensure.Positive(newQuantity, nameof(newQuantity));
    }

    public void UpdateReason(string reason)
    {
        Reason = Ensure.NotEmpty(reason, nameof(reason));
    }

    public void SetInstalledBy(string installedBy)
    {
        InstalledBy = Ensure.NotEmpty(installedBy, nameof(installedBy));
    }
}
