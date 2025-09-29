using ECommerceBackend.Domain.Abstracts;
using MediatR;

namespace ECommerceBackend.Application.Abstracts.Messaging;


/// HDHiep - 09/24/2025
/// <summary>
/// MediatR request handler contracts for commands returning <see cref="Result"/> or <see cref="Result{TValue}"/>.
/// </summary>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>;
