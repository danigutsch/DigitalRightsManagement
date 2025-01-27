using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Domain.ProductAggregate.Events;

namespace DigitalRightsManagement.Application.AgentAggregate.DomainEventHandlers;

public sealed class WorkerAssignedDomainEventHandler(IAgentRepository agentRepository) : IDomainEventHandler<WorkerAssigned>
{
    public async Task Handle(WorkerAssigned notification, CancellationToken cancellationToken)
    {
        await agentRepository.GetById(notification.WorkerId, cancellationToken)
            .BindAsync(t => t.AddProduct(notification.ProductId))
            .Tap(() => agentRepository.UnitOfWork.SaveEntities(cancellationToken));
    }
}
