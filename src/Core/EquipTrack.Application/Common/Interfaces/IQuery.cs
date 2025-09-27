using MediatR;
using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Application.Common.Interfaces;

/// <summary>
/// Marker interface for queries that return a result.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse>
    where TResponse : IResult
{
}