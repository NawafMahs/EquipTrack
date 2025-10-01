using EquipTrack.Domain.Common;

namespace EquipTrack.Domain.Entities;

/// <summary>
/// Tracks spare part usage for robots.
/// Links robots to spare parts with usage details.
/// </summary>
public class RobotSparePartUsage : BaseEntity
{
    public Guid RobotRef { get; init; }
    public Guid SparePartRef { get; init; }
    public int QuantityUsed { get; private set; }
    public DateTime UsageDate { get; init; }
    public string? Reason { get; private set; }
    public Guid? WorkOrderRef { get; init; }
    public string? InstalledBy { get; private set; }

    // EF Core constructor
    protected RobotSparePartUsage() { }

    private RobotSparePartUsage(
        Guid robotRef,
        Guid sparePartRef,
        int quantityUsed,
        DateTime usageDate,
        string? reason = null,
        Guid? workOrderRef = null,
        string? installedBy = null)
    {
        RobotRef = robotRef;
        SparePartRef = sparePartRef;
        QuantityUsed = Ensure.Positive(quantityUsed, nameof(quantityUsed));
        UsageDate = usageDate;
        Reason = reason;
        WorkOrderRef = workOrderRef;
        InstalledBy = installedBy;
    }

    public static RobotSparePartUsage Create(
        Guid robotRef,
        Guid sparePartRef,
        int quantityUsed,
        DateTime usageDate,
        string? reason = null,
        Guid? workOrderRef = null,
        string? installedBy = null)
    {
        return new RobotSparePartUsage(robotRef, sparePartRef, quantityUsed, usageDate, reason, workOrderRef, installedBy);
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
