using MediatR;
using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Application.WorkOrders.Commands;

public sealed record CompleteWorkOrderCommand(
    Guid WorkOrderId, 
    string? CompletionNotes, 
    decimal ActualHours, 
    decimal ActualCost) : IRequest<Result<bool>>;
