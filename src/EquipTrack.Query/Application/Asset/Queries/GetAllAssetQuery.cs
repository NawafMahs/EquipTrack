using EquipTrack.Core.SharedKernel;
using EquipTrack.Query.QueryModels;
using MediatR;

namespace EquipTrack.Application.Assets.Queries;

public record GetAllAssetQuery : IRequest<Result<List<AssetQueryModel>>>;
