using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Domain.ProductAggregate.Events;

namespace DigitalRightsManagement.Application.AgentAggregate.DomainEventHandlers;

public sealed class ProductCreatedDomainEventHandler(IAgentRepository agentRepository) : IDomainEventHandler<ProductCreated>
{
    public async Task Handle(ProductCreated notification, CancellationToken cancellationToken)
    {
        await agentRepository.GetById(notification.AgentId, cancellationToken)
            .Tap(t => t.AddProduct(notification.ProductId))
            .Tap(_ => agentRepository.UnitOfWork.SaveEntities(cancellationToken));
    }
}
