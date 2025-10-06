using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Infrastructure.Data.Configurations;

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.HasKey(a => a.Id);

        // Configure TPH (Table-Per-Hierarchy) inheritance
        builder.HasDiscriminator(a => a.AssetType)
            .HasValue<Machine>(AssetType.Machine)
            .HasValue<Robot>(AssetType.Robot);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Description)
            .HasMaxLength(1000);

        builder.Property(a => a.SerialNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(a => a.SerialNumber)
            .IsUnique();

        builder.Property(a => a.Model)
            .HasMaxLength(100);

        builder.Property(a => a.Manufacturer)
            .HasMaxLength(100);

        builder.Property(a => a.Location)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(a => a.PurchasePrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(a => a.ImageUrl)
            .HasMaxLength(500);

        builder.Property(a => a.Notes)
            .HasMaxLength(2000);

        // Configure Metadata as JSON column
        builder.Property(a => a.Metadata)
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>()
            );

        // Navigation properties
        builder.HasMany(a => a.WorkOrders)
            .WithOne(wo => wo.Asset)
            .HasForeignKey(wo => wo.AssetRef)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.PreventiveMaintenances)
            .WithOne(pm => pm.Asset)
            .HasForeignKey(pm => pm.AssetRef)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore navigation properties that are not yet implemented in DbContext
        builder.Ignore(a => a.Logs);
        builder.Ignore(a => a.Sensors);
        builder.Ignore(a => a.SparePartUsages);
        builder.Ignore(a => a.SensorReadings);
    }
}