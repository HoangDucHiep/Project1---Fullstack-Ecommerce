using ECommerceBackend.Domain.Abstracts;
using MediatR;

namespace ECommerceBackend.Application.Abstracts.Messaging;

/// HDHiep - 09/24/2025
/// <summary>
/// MediatR notification handler contract for handling domain events.
/// </summary>
public interface IDomainEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent;
