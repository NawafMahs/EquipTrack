using Microsoft.EntityFrameworkCore;

namespace EquipTrack.Infrastructure.Extensions;

internal static class ModelBuilderExtensions
{
    internal static void RemoveCascadeDeleteConvention(this ModelBuilder modelBuilder)
    {
        var cascadeFKs = modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)
            .ToList();
        foreach (var fk in cascadeFKs)
        {
            fk.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}
