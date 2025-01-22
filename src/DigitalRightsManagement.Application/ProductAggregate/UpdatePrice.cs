using Ardalis.Result;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Common.Messaging;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Application.ProductAggregate;

public sealed record UpdatePriceCommand(Guid ProductId, decimal NewPrice, Currency Currency, string Reason) : ICommand
{
    internal sealed class UpdatePriceCommandHandler(ICurrentUserProvider currentUserProvider, IProductRepository productRepository) : ICommandHandler<UpdatePriceCommand>
    {
        public async Task<Result> Handle(UpdatePriceCommand command, CancellationToken cancellationToken)
        {
            var userResult = await currentUserProvider.Get(cancellationToken);
            if (!userResult.IsSuccess)
            {
                return userResult.Map();
            }

            var user = userResult.Value;

            var productResult = await productRepository.GetById(command.ProductId, cancellationToken);
            if (!productResult.IsSuccess)
            {
                return productResult.Map();
            }

            var product = productResult.Value;

            return await Price.Create(command.NewPrice, command.Currency)
                .Tap(price => product.UpdatePrice(user.Id, price, command.Reason))
                .Tap(_ => productRepository.UnitOfWork.SaveEntities(cancellationToken))
                .MapAsync(_ => Result.Success());
        }
    }
}
