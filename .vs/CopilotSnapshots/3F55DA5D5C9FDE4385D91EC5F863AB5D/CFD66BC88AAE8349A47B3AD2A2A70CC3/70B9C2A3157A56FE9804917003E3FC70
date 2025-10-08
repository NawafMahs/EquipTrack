using MediatR;

namespace EquipTrack.Core.SharedKernel;



/// <summary>
/// Represents a repository for storing events in an event store.
/// </summary>
public abstract class BaseEvent : INotification
{
    protected BaseEvent()
    {
        
    }
    public string MesageType { get; protected init; } = default!;
    public Guid AggregateId { get; protected init; }
    public DateTime OccurredOn { get; protected init; } = DateTime.Now;
}
