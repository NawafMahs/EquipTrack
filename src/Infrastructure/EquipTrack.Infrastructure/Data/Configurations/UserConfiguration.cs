using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(u => u.Role)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        // Navigation properties
        builder.HasMany(u => u.AssignedWorkOrders)
            .WithOne(wo => wo.AssignedToUser)
            .HasForeignKey(wo => wo.AssignedToUserRef)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(u => u.CreatedWorkOrders)
            .WithOne(wo => wo.CreatedByUser)
            .HasForeignKey(wo => wo.CreatedByUserRef)
            .OnDelete(DeleteBehavior.Restrict);

        // Ignore computed property
        builder.Ignore(u => u.FullName);
    }
}