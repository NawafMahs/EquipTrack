using MediatR;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Application.Assets.Commands;

/// <summary>
/// Command to update an asset's status.
/// </summary>
public sealed record UpdateAssetStatusCommand : IRequest<Result<Guid>>
{
    /// <summary>
    /// Initializes a new instance of <see cref="UpdateAssetStatusCommand"/>.
    /// </summary>
    /// <param name="id">Asset ID.</param>
    /// <param name="status">New asset status.</param>
    public UpdateAssetStatusCommand(Guid id, AssetStatus status)
    {
        Id = id;
        Status = status;
    }

    /// <summary>
    /// Asset ID.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// New asset status.
    /// </summary>
    public AssetStatus Status { get; init; }
}