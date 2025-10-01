using MediatR;
using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Application.WorkOrders.Commands;

public sealed record AssignWorkOrderCommand(Guid WorkOrderId, Guid UserId) : IRequest<Result<bool>>;
