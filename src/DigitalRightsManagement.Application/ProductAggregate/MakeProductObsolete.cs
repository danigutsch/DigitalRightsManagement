using Ardalis.Result;
using DigitalRightsManagement.Application.Authorization;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Common.Messaging;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Application.ProductAggregate;

[AuthorizeResourceOwner<Product>("Id")]
public sealed record MakeProductObsoleteCommand(Guid Id) : ICommand
{
    internal sealed class MakeProductObsoleteCommandHandler(ICurrentAgentProvider currentAgentProvider, IProductRepository productRepository) : ICommandHandler<MakeProductObsoleteCommand>
    {
        public async Task<Result> Handle(MakeProductObsoleteCommand command, CancellationToken cancellationToken)
        {
            var agentResult = await currentAgentProvider.Get(cancellationToken);
            if (!agentResult.IsSuccess)
            {
                return agentResult.Map();
            }

            var agent = agentResult.Value;

            return await productRepository.GetById(ProductId.From(command.Id), cancellationToken)
                .BindAsync(product => product.Obsolete(agent.Id))
                .Tap(() => productRepository.UnitOfWork.SaveEntities(cancellationToken));
        }
    }
}
