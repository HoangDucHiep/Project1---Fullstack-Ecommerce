using ECommerceBackend.Common.Domain;
using MediatR;

namespace ECommerceBackend.Common.Application.Messaging;
public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
