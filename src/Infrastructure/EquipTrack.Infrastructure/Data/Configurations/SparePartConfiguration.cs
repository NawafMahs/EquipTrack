using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EquipTrack.Domain.Entities;

namespace EquipTrack.Infrastructure.Data.Configurations;

public class SparePartConfiguration : IEntityTypeConfiguration<SparePart>
{
    public void Configure(EntityTypeBuilder<SparePart> builder)
    {
        builder.HasKey(sp => sp.Id);

        builder.Property(sp => sp.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(sp => sp.Description)
            .HasMaxLength(1000);

        builder.Property(sp => sp.PartNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(sp => sp.PartNumber)
            .IsUnique();

        builder.Property(sp => sp.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sp => sp.Supplier)
            .HasMaxLength(200);

        builder.Property(sp => sp.UnitPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(sp => sp.Unit)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(sp => sp.Location)
            .HasMaxLength(200);

        builder.Property(sp => sp.Notes)
            .HasMaxLength(1000);

        // Navigation properties
        builder.HasMany(sp => sp.WorkOrderSpareParts)
            .WithOne(wosp => wosp.SparePart)
            .HasForeignKey(wosp => wosp.SparePartRef)
            .OnDelete(DeleteBehavior.Restrict);

        // Ignore computed property
        builder.Ignore(sp => sp.IsLowStock);
    }
}