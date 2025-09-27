using MediatR;
using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Application.Common.Interfaces;

/// <summary>
/// Marker interface for commands that return a result.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>
    where TResponse : IResult
{
}

/// <summary>
/// Marker interface for commands that don't return a value.
/// </summary>
public interface ICommand : IRequest<Result>
{
}