namespace EquipTrack.Core.SharedKernel;

public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Saves the changes made in the unit of work asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveChangesAsync(CancellationToken token);
}
