using Ardalis.Result;
using DigitalRightsManagement.Application.Authorization;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Common.Messaging;
using DigitalRightsManagement.Domain.AgentAggregate;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Application.ProductAggregate;

[AuthorizeResourceOwner<Product>("Id")]
public sealed record UnassignWorkerCommand(Guid Id, Guid WorkerId) : ICommand
{
    internal sealed class UnassignWorkerCommandHandler(
        ICurrentAgentProvider currentAgentProvider,
        IProductRepository productRepository) : ICommandHandler<UnassignWorkerCommand>
    {
        public async Task<Result> Handle(UnassignWorkerCommand command, CancellationToken cancellationToken)
        {
            var productId = ProductId.From(command.Id);
            var workerId = AgentId.From(command.WorkerId);

            var currentAgentResult = await currentAgentProvider.Get(cancellationToken);
            if (!currentAgentResult.IsSuccess)
            {
                return currentAgentResult.Map();
            }

            var currentAgent = currentAgentResult.Value;

            return await productRepository.GetById(productId, cancellationToken)
                .BindAsync(product => product.UnassignWorker(currentAgent.Id, workerId))
                .Tap(() => productRepository.UnitOfWork.SaveEntities(cancellationToken));
        }
    }
}
