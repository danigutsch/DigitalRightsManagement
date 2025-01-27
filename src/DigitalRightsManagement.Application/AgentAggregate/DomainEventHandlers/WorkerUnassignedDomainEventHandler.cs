using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Domain.ProductAggregate.Events;

namespace DigitalRightsManagement.Application.AgentAggregate.DomainEventHandlers;

public sealed class WorkerUnassignedDomainEventHandler(IAgentRepository agentRepository) : IDomainEventHandler<WorkerUnassigned>
{
    public async Task Handle(WorkerUnassigned notification, CancellationToken cancellationToken)
    {
        await agentRepository.GetById(notification.WorkerId, cancellationToken)
            .BindAsync(t => t.RemoveProduct(notification.ProductId))
            .Tap(() => agentRepository.UnitOfWork.SaveEntities(cancellationToken));
    }
}