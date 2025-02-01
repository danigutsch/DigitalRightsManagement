using Ardalis.Result;
using DigitalRightsManagement.Application.Authorization;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Common.Messaging;
using DigitalRightsManagement.Domain;
using DigitalRightsManagement.Domain.AgentAggregate;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Application.ProductAggregate;

[AuthorizeResourceOwner<Product>]
public sealed record AssignWorkerCommand(Guid ProductId, Guid WorkerId) : ICommand
{
    internal sealed class AssignWorkerCommandHandler(
        ICurrentAgentProvider currentUserProvider,
        IAgentRepository userRepository,
        IProductRepository productRepository) : ICommandHandler<AssignWorkerCommand>
    {
        public async Task<Result> Handle(AssignWorkerCommand command, CancellationToken cancellationToken)
        {
            var currentAgentResult = await currentUserProvider.Get(cancellationToken);
            if (!currentAgentResult.TryGetValue(out var currentAgent))
            {
                return currentAgentResult.Map();
            }

            var workerResult = await userRepository.GetById(command.WorkerId, cancellationToken);
            if (!workerResult.TryGetValue(out var worker))
            {
                return workerResult.Map();
            }

            if (worker.Role != AgentRoles.Worker)
            {
                return Errors.Agents.InvalidWorkerRole(worker.Id, worker.Role);
            }

            return await productRepository.GetById(command.ProductId, cancellationToken)
                .BindAsync(product => product.AssignWorker(currentAgent.Id, worker.Id))
                .Tap(() => productRepository.UnitOfWork.SaveEntities(cancellationToken));
        }
    }
}
