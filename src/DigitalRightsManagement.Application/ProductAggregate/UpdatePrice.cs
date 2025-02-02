using Ardalis.Result;
using DigitalRightsManagement.Application.Authorization;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Common.Messaging;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Application.ProductAggregate;

[AuthorizeResourceOwner<Product>("Id")]
public sealed record UpdatePriceCommand(Guid Id, decimal NewPrice, Currency Currency, string Reason) : ICommand
{
    internal sealed class UpdatePriceCommandHandler(ICurrentAgentProvider currentAgentProvider, IProductRepository productRepository) : ICommandHandler<UpdatePriceCommand>
    {
        public async Task<Result> Handle(UpdatePriceCommand command, CancellationToken cancellationToken)
        {
            var productId = ProductId.From(command.Id);

            var agentResult = await currentAgentProvider.Get(cancellationToken);
            if (!agentResult.IsSuccess)
            {
                return agentResult.Map();
            }

            var agent = agentResult.Value;

            var productResult = await productRepository.GetById(productId, cancellationToken);
            if (!productResult.IsSuccess)
            {
                return productResult.Map();
            }

            var product = productResult.Value;

            return await Price.From(command.NewPrice, command.Currency)
                .Tap(price => product.UpdatePrice(agent.Id, price, command.Reason))
                .Tap(_ => productRepository.UnitOfWork.SaveEntities(cancellationToken))
                .MapAsync(_ => Result.Success());
        }
    }
}
