using ECommerceBackend.Domain.Abstracts;
using MediatR;

namespace ECommerceBackend.Application.Abstracts.Messaging;

/// HDHiep - 09/24/2025
/// <summary>
/// MediatR request handler contract for queries that return a <see cref="Result{TValue}"/>.
/// </summary>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;
