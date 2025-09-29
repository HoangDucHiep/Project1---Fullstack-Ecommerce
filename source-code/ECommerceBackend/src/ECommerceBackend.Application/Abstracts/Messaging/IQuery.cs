using ECommerceBackend.Domain.Abstracts;
using MediatR;

namespace ECommerceBackend.Application.Abstracts.Messaging;

/// HDHiep - 09/24/2025
/// <summary>
/// Marker request type for queries that return <see cref="Result{TValue}"/> via MediatR.
/// </summary>
public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
