using Ardalis.Result;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Common.Messaging;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Application.ProductAggregate;

internal sealed class CreateProductCommandHandler(ICurrentUserProvider currentUserProvider, IProductRepository productRepository) : ICommandHandler<CreateProductCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var userResult = await currentUserProvider.Get(cancellationToken);
        if (!userResult.IsSuccess)
        {
            return userResult.Map();
        }

        var user = userResult.Value;

        return await Price.Create(command.Price, command.Currency)
            .Bind(price => Product.Create(command.Name, command.Description, price, user.Id))
            .Tap(productRepository.Add)
            .Tap(_ => productRepository.UnitOfWork.SaveEntities(cancellationToken))
            .MapAsync(product => product.Id);
    }
}

public sealed record CreateProductCommand(string Name, string Description, decimal Price, Currency Currency) : ICommand<Guid>;
