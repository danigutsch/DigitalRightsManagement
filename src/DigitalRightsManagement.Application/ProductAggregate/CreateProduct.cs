using Ardalis.Result;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Common.Messaging;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Application.ProductAggregate;

public sealed class CreateProductCommandHandler(IUserRepository userRepository, IProductRepository productRepository, IUnitOfWork unitOfWork) : ICommandHandler<CreateProductCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        return await userRepository.GetById(command.UserId, cancellationToken)
            .DoubleBind(user => Price.Create(command.Price, command.Currency))
            .BindAsync(t => Product.Create(command.Name, command.Description, t.Next, t.Prev.Id))
            .Tap(productRepository.Add)
            .Tap(_ => unitOfWork.SaveChanges(cancellationToken))
            .MapAsync(product => product.Id);
    }
}

public sealed record CreateProductCommand(Guid UserId, string Name, string Description, decimal Price, Currency Currency) : ICommand<Guid>;
