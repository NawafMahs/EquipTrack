using Microsoft.EntityFrameworkCore;
using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Common;

namespace EquipTrack.Infrastructure.Data;

public class EquipTrackDbContext : DbContext
{
    public EquipTrackDbContext(DbContextOptions<EquipTrackDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<WorkOrder> WorkOrders { get; set; }
    public DbSet<SparePart> SpareParts { get; set; }
    public DbSet<WorkOrderSparePart> WorkOrderSpareParts { get; set; }
    public DbSet<PreventiveMaintenance> PreventiveMaintenances { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EquipTrackDbContext).Assembly);

        // Global query filters for soft delete (if needed in future)
        // modelBuilder.Entity<BaseEntity>().HasQueryFilter(e => !e.IsDeleted);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity && (
                e.State == EntityState.Added ||
                e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            var entity = (BaseEntity)entityEntry.Entity;

            if (entityEntry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entityEntry.State == EntityState.Modified)
            {
                entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}