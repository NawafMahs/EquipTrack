using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EquipTrack.Domain.Entities;

namespace EquipTrack.Infrastructure.Data.Configurations;

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.HasKey(a => a.Id);

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

        // Navigation properties
        builder.HasMany(a => a.WorkOrders)
            .WithOne(wo => wo.Asset)
            .HasForeignKey(wo => wo.AssetRef)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.PreventiveMaintenances)
            .WithOne(pm => pm.Asset)
            .HasForeignKey(pm => pm.AssetRef)
            .OnDelete(DeleteBehavior.Cascade);
    }
}