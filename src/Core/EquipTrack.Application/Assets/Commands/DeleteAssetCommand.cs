using MediatR;
using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Application.Assets.Commands;

/// <summary>
/// Command to delete an asset.
/// </summary>
public sealed record DeleteAssetCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// Initializes a new instance of <see cref="DeleteAssetCommand"/>.
    /// </summary>
    /// <param name="id">Asset ID.</param>
    public DeleteAssetCommand(Guid id)
    {
        Id = id;
    }

    /// <summary>
    /// Asset ID.
    /// </summary>
    public Guid Id { get; init; }
}