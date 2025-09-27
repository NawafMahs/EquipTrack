using MediatR;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Application.DTOs;

namespace EquipTrack.Application.Assets.Queries;

/// <summary>
/// Query to get an asset by its ID.
/// </summary>
public sealed record GetAssetByIdQuery : IRequest<Result<AssetQuery>>
{
    /// <summary>
    /// Initializes a new instance of <see cref="GetAssetByIdQuery"/>.
    /// </summary>
    /// <param name="id">Asset ID.</param>
    public GetAssetByIdQuery(Guid id)
    {
        Id = id;
    }

    /// <summary>
    /// Asset ID.
    /// </summary>
    public Guid Id { get; init; }
}