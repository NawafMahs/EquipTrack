using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EquipTrack.Domain.Entities;

namespace EquipTrack.Infrastructure.Data.Configurations;

public class PreventiveMaintenanceConfiguration : IEntityTypeConfiguration<PreventiveMaintenance>
{
    public void Configure(EntityTypeBuilder<PreventiveMaintenance> builder)
    {
        builder.HasKey(pm => pm.Id);

        builder.Property(pm => pm.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(pm => pm.Description)
            .HasMaxLength(1000);

        builder.Property(pm => pm.Frequency)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(pm => pm.FrequencyValue)
            .IsRequired();

        builder.Property(pm => pm.IsActive)
            .HasDefaultValue(true);

        builder.Property(pm => pm.EstimatedHours)
            .HasColumnType("decimal(8,2)");

        builder.Property(pm => pm.EstimatedCost)
            .HasColumnType("decimal(18,2)");

        builder.Property(pm => pm.Instructions)
            .HasMaxLength(2000);

        // Navigation properties
        builder.HasOne(pm => pm.Asset)
            .WithMany(a => a.PreventiveMaintenances)
            .HasForeignKey(pm => pm.AssetRef)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pm => pm.AssignedToUser)
            .WithMany()
            .HasForeignKey(pm => pm.AssignedToUserRef)
            .OnDelete(DeleteBehavior.SetNull);

        // Ignore computed properties
        builder.Ignore(pm => pm.IsOverdue);
        builder.Ignore(pm => pm.IsDueSoon);

        // Index for performance
        builder.HasIndex(pm => pm.NextDueDate);
        builder.HasIndex(pm => new { pm.AssetRef, pm.IsActive });
    }
}