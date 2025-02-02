using Ardalis.Result;
using DigitalRightsManagement.Application.Authorization;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Common.Messaging;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Application.ProductAggregate;

[AuthorizeResourceOwner<Product>("Id")]
public sealed record UpdateDescriptionCommand(Guid Id, string NewDescription) : ICommand
{
    internal sealed class UpdateDescriptionCommandHandler(ICurrentAgentProvider currentAgentProvider, IProductRepository productRepository) : ICommandHandler<UpdateDescriptionCommand>
    {
        public async Task<Result> Handle(UpdateDescriptionCommand command, CancellationToken cancellationToken)
        {
            var productId = ProductId.From(command.Id);
            var descriptionResult = Description.From(command.NewDescription);
            if (!descriptionResult.TryGetValue(out var description))
            {
                return descriptionResult.Map();
            }

            var agentResult = await currentAgentProvider.Get(cancellationToken);
            if (!agentResult.TryGetValue(out var agent))
            {
                return agentResult.Map();
            }

            return await productRepository.GetById(productId, cancellationToken)
                .BindAsync(product => product.UpdateDescription(agent.Id, description)
                .Tap(_ => productRepository.UnitOfWork.SaveEntities(cancellationToken))
                .MapAsync(_ => Result.Success()));
        }
    }
}
