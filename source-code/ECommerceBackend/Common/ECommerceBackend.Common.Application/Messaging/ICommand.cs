using ECommerceBackend.Common.Domain;
using MediatR;

namespace ECommerceBackend.Common.Application.Messaging;
/// <summary>
/// Marker request types for commands handled via MediatR that return <see cref="Result"/> or <see cref="Result{TValue}"/>.
/// </summary>
public interface ICommand : IRequest<Result>, IBaseCommand;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand;

/// <summary>
/// Marker interface for all commands.
/// </summary>
public interface IBaseCommand;
