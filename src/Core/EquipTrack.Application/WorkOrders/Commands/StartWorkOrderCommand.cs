using MediatR;
using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Application.WorkOrders.Commands;

public sealed record StartWorkOrderCommand(Guid WorkOrderId) : IRequest<Result<bool>>;
