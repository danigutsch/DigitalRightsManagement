using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Domain.ProductAggregate.Events;

namespace DigitalRightsManagement.Application.UserAggregate.DomainEventHandlers;

public sealed class ProductCreatedDomainEventHandler(IUserRepository userRepository) : IDomainEventHandler<ProductCreated>
{
    public async Task Handle(ProductCreated notification, CancellationToken cancellationToken)
    {
        await userRepository.GetById(notification.UserId, cancellationToken)
            .Tap(t => t.AddProduct(notification.ProductId))
            .Tap(_ => userRepository.UnitOfWork.SaveEntities(cancellationToken));
    }
}
