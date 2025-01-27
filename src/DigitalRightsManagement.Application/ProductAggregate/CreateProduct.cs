using Ardalis.Result;
using DigitalRightsManagement.Application.Authorization;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Common.Messaging;
using DigitalRightsManagement.Domain.AgentAggregate;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Application.ProductAggregate;

[Authorize(AgentRoles.Manager)]
public sealed record CreateProductCommand(string Name, string Description, decimal Price, Currency Currency) : ICommand<Guid>
{
    internal sealed class CreateProductCommandHandler(ICurrentAgentProvider currentAgentProvider, IProductRepository productRepository) : ICommandHandler<CreateProductCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var agentResult = await currentAgentProvider.Get(cancellationToken);
            if (!agentResult.IsSuccess)
            {
                return agentResult.Map();
            }

            var agent = agentResult.Value;

            return await Domain.ProductAggregate.Price.Create(command.Price, command.Currency)
                .Bind(price => Product.Create(command.Name, command.Description, price, agent.Id))
                .Tap(productRepository.Add)
                .Tap(_ => productRepository.UnitOfWork.SaveEntities(cancellationToken))
                .MapAsync(product => product.Id);
        }
    }
}
