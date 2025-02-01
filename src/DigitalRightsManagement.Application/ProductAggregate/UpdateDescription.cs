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
            var newDescriptionResult = Description.From(command.NewDescription);
            if (!newDescriptionResult.TryGetValue(out var newDescription))
            {
                return newDescriptionResult.Map();
            }

            var agentResult = await currentAgentProvider.Get(cancellationToken);
            if (!agentResult.TryGetValue(out var agent))
            {
                return agentResult.Map();
            }

            return await productRepository.GetById(command.ProductId, cancellationToken)
                .BindAsync(product => product.UpdateDescription(agent.Id, newDescription)
                .Tap(_ => productRepository.UnitOfWork.SaveEntities(cancellationToken))
                .MapAsync(_ => Result.Success()));
        }
    }
}
