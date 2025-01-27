using Ardalis.Result;
using DigitalRightsManagement.Application.Authorization;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Common.Messaging;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Application.ProductAggregate;

[AuthorizeResourceOwner<Product>]
public sealed record UpdateDescriptionCommand(Guid ProductId, string NewDescription) : ICommand
{
    internal sealed class UpdateDescriptionCommandHandler(ICurrentAgentProvider currentAgentProvider, IProductRepository productRepository) : ICommandHandler<UpdateDescriptionCommand>
    {
        public async Task<Result> Handle(UpdateDescriptionCommand command, CancellationToken cancellationToken)
        {
            var agentResult = await currentAgentProvider.Get(cancellationToken);
            if (!agentResult.IsSuccess)
            {
                return agentResult.Map();
            }

            var agent = agentResult.Value;

            var productResult = await productRepository.GetById(command.ProductId, cancellationToken);
            if (!productResult.IsSuccess)
            {
                return productResult.Map();
            }

            var product = productResult.Value;


            return await product.UpdateDescription(agent.Id, command.NewDescription)
                .Tap(_ => productRepository.UnitOfWork.SaveEntities(cancellationToken))
                .MapAsync(_ => Result.Success());
        }
    }
}
