using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EquipTrack.Domain.Entities;

namespace EquipTrack.Infrastructure.Data.Configurations;

public class WorkOrderConfiguration : IEntityTypeConfiguration<WorkOrder>
{
    public void Configure(EntityTypeBuilder<WorkOrder> builder)
    {
        builder.HasKey(wo => wo.Id);

        builder.Property(wo => wo.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(wo => wo.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(wo => wo.Priority)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(wo => wo.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(wo => wo.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(wo => wo.EstimatedHours)
            .HasColumnType("decimal(8,2)");

        builder.Property(wo => wo.ActualHours)
            .HasColumnType("decimal(8,2)");

        builder.Property(wo => wo.EstimatedCost)
            .HasColumnType("decimal(18,2)");

        builder.Property(wo => wo.ActualCost)
            .HasColumnType("decimal(18,2)");

        builder.Property(wo => wo.CompletionNotes)
            .HasMaxLength(2000);

        // Navigation properties
        builder.HasOne(wo => wo.Asset)
            .WithMany(a => a.WorkOrders)
            .HasForeignKey(wo => wo.AssetRef)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(wo => wo.CreatedByUser)
            .WithMany(u => u.CreatedWorkOrders)
            .HasForeignKey(wo => wo.CreatedByUserRef)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(wo => wo.AssignedToUser)
            .WithMany(u => u.AssignedWorkOrders)
            .HasForeignKey(wo => wo.AssignedToUserRef)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(wo => wo.WorkOrderSpareParts)
            .WithOne(wosp => wosp.WorkOrder)
            .HasForeignKey(wosp => wosp.WorkOrderRef)
            .OnDelete(DeleteBehavior.Cascade);
    }
}