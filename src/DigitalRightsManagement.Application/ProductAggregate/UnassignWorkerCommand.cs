using Ardalis.Result;
using DigitalRightsManagement.Application.Authorization;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Common.Messaging;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Application.ProductAggregate;

[AuthorizeResourceOwner<Product>]
public sealed record UnassignWorkerCommand(Guid ProductId, Guid WorkerId) : ICommand
{
    internal sealed class UnassignWorkerCommandHandler(
        ICurrentAgentProvider currentAgentProvider,
        IProductRepository productRepository) : ICommandHandler<UnassignWorkerCommand>
    {
        public async Task<Result> Handle(UnassignWorkerCommand command, CancellationToken cancellationToken)
        {
            var currentAgentResult = await currentAgentProvider.Get(cancellationToken);
            if (!currentAgentResult.IsSuccess)
            {
                return currentAgentResult.Map();
            }

            var currentAgent = currentAgentResult.Value;

            return await productRepository.GetById(command.ProductId, cancellationToken)
                .BindAsync(product => product.UnassignWorker(currentAgent.Id, command.WorkerId))
                .Tap(() => productRepository.UnitOfWork.SaveEntities(cancellationToken));
        }
    }
}
