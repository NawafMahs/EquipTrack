using EquipTrack.Domain.Entities;

namespace EquipTrack.Infrastructure.Mappings;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class WorkOrderConfiguration : IEntityTypeConfiguration<WorkOrder>
{
    public void Configure(EntityTypeBuilder<WorkOrder> builder)
    {
        // ==========================
        // Table Configuration
        // ==========================
        builder.ToTable("WorkOrders", tb =>
        {
            tb.HasComment("Stores all work orders within the CMMS system");
            tb.HasTrigger("TR_WorkOrders_SetUpdatedAt");
            tb.HasTrigger("TR_WorkOrders_AuditChange");
        });

        // ==========================
        // Primary Key
        // ==========================
        builder.HasKey(w => w.Id);

        // ==========================
        // Core Properties
        // ==========================
        builder.Property(w => w.Title)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("Short descriptive title of the work order");

        builder.Property(w => w.Description)
            .HasMaxLength(2000)
            .HasComment("Detailed description of the work order");

        builder.Property(w => w.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasComment("Current status of the work order");

        builder.Property(w => w.Priority)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasComment("Priority level of the work order");

        builder.Property(w => w.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasComment("Type of work order (Corrective, Preventive, etc.)");

        builder.Property(w => w.RequestedDate)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(w => w.ScheduledDate)
            .HasColumnType("datetime2");

        builder.Property(w => w.StartedDate)
            .HasColumnType("datetime2");

        builder.Property(w => w.CompletedDate)
            .HasColumnType("datetime2");

        builder.Property(w => w.CompletionNotes)
            .HasMaxLength(2000);

        builder.Property(w => w.EstimatedHours)
            .HasPrecision(10, 2);

        builder.Property(w => w.ActualHours)
            .HasPrecision(10, 2);

        builder.Property(w => w.EstimatedCost)
            .HasPrecision(18, 2);

        builder.Property(w => w.ActualCost)
            .HasPrecision(18, 2);

        // ==========================
        // Relationships
        // ==========================
        builder.HasOne(w => w.Asset)
            .WithMany(a => a.WorkOrders)
            .HasForeignKey(w => w.AssetRef)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_WorkOrders_Assets");

        builder.HasOne(w => w.CreatedByUser)
            .WithMany()
            .HasForeignKey(w => w.CreatedByUserRef)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_WorkOrders_CreatedByUser");

        builder.HasOne(w => w.AssignedToUser)
            .WithMany()
            .HasForeignKey(w => w.AssignedToUserRef)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_WorkOrders_AssignedToUser");

        builder.Metadata
            .FindNavigation(nameof(WorkOrder.WorkOrderSpareParts))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // ==========================
        // Indexes (Valid + Optimized)
        // ==========================
        builder.HasIndex(w => new { w.Status, w.Priority })
            .HasDatabaseName("IX_WorkOrders_Status_Priority")
            .HasFilter("[Status] <> 'Cancelled'");

        builder.HasIndex(w => w.AssetRef)
            .HasDatabaseName("IX_WorkOrders_AssetRef");

        builder.HasIndex(w => w.RequestedDate)
            .HasDatabaseName("IX_WorkOrders_RequestedDate");

        builder.HasIndex(w => w.AssignedToUserRef)
            .HasDatabaseName("IX_WorkOrders_AssignedToUser");

        // ==========================
        // Auditing Fields
        // ==========================
        builder.Property<DateTime>("CreatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property<DateTime?>("UpdatedAt")
            .HasColumnType("datetime2");
    }
}
