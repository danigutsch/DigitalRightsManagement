using Ardalis.Result;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Common.Messaging;
using DigitalRightsManagement.Domain.ProductAggregate;
using DigitalRightsManagement.Infrastructure.Identity;

namespace DigitalRightsManagement.Application.ProductAggregate;

public sealed class CreateProductCommandHandler(ICurrentUserProvider currentUserProvider, IProductRepository productRepository, IUnitOfWork unitOfWork) : ICommandHandler<CreateProductCommand, Guid>
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
            .Tap(product => user.AddProduct(product))
            .Tap(productRepository.Add)
            .Tap(_ => unitOfWork.SaveChanges(cancellationToken))
            .MapAsync(product => product.Id);
    }
}

public sealed record CreateProductCommand(string Name, string Description, decimal Price, Currency Currency) : ICommand<Guid>;
