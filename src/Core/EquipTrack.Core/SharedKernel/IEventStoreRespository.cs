namespace EquipTrack.Core.SharedKernel;

public interface IEventStoreRespository
{
    /// <summary>
    /// Stores a collection of event stores asynchronously.
    /// </summary>
    /// <param name="eventStores">The event stores to store.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task StoreAsync(IEnumerable<EventStore> eventStores);
}