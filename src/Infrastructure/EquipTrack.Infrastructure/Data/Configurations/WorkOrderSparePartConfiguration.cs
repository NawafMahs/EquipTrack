using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EquipTrack.Domain.Entities;

namespace EquipTrack.Infrastructure.Data.Configurations;

public class WorkOrderSparePartConfiguration : IEntityTypeConfiguration<WorkOrderSparePart>
{
    public void Configure(EntityTypeBuilder<WorkOrderSparePart> builder)
    {
        builder.HasKey(wosp => wosp.Id);

        builder.Property(wosp => wosp.QuantityUsed)
            .IsRequired();

        builder.Property(wosp => wosp.UnitCost)
            .HasColumnType("decimal(18,2)");

        builder.Property(wosp => wosp.Notes)
            .HasMaxLength(500);

        // Navigation properties
        builder.HasOne(wosp => wosp.WorkOrder)
            .WithMany(wo => wo.WorkOrderSpareParts)
            .HasForeignKey(wosp => wosp.WorkOrderRef)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(wosp => wosp.SparePart)
            .WithMany(sp => sp.WorkOrderSpareParts)
            .HasForeignKey(wosp => wosp.SparePartRef)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(wosp => wosp.Asset)
            .WithMany() // Asset doesn't have WorkOrderSpareParts collection
            .HasForeignKey(wosp => wosp.AssetRef)
            .OnDelete(DeleteBehavior.SetNull);

        // Ignore computed property
        builder.Ignore(wosp => wosp.TotalCost);

        // Composite index for performance
        builder.HasIndex(wosp => new { wosp.WorkOrderRef, wosp.SparePartRef });
    }
}