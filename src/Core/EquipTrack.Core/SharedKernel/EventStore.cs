namespace EquipTrack.Core.SharedKernel;

public class EventStore : BaseEvent
{
    public EventStore(Guid aggregatedId, string messageType, Guid aggregateId)
    {
        AggregateId = aggregateId;
        MesageType = messageType;
        AggregateId = aggregateId;
    }
    public EventStore()
    {

    }
    public Guid Id { get; private init; } = Guid.NewGuid();
    public string Data { get; private init; } = default!;
}