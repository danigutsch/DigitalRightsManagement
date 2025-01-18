using MediatR;

namespace DigitalRightsManagement.Application.Messaging;

internal interface IDomainEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent> where TDomainEvent : INotification;
