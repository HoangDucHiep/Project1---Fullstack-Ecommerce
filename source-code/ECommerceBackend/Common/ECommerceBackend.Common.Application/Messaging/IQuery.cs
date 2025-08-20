using ECommerceBackend.Common.Domain;
using MediatR;

namespace ECommerceBackend.Common.Application.Messaging;
/// <summary>
/// Marker request type for queries that return <see cref="Result{TValue}"/> via MediatR.
/// </summary>
public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
