using DigitalRightsManagement.Common;
using Ardalis.Result;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Application.ProductAggregate;

public sealed class CreateProductCommandHandler(IUserRepository userRepository, IProductRepository productRepository, IUnitOfWork unitOfWork)
{
    public async Task<Result> Handle(CreateProductCommand command, CancellationToken ct)
    {
        return await userRepository.GetById(command.UserId, ct)
            .DoubleBind(user => Price.Create(command.Price, command.Currency))
            .BindAsync(t => Product.Create(command.Name, command.Description, t.Next, t.Prev.Id))
            .Tap(productRepository.Add)
            .Tap(_ => unitOfWork.SaveChanges(ct))
            .MapAsync(_ => Result.Success());
    }
}

public sealed record CreateProductCommand(Guid UserId, string Name, string Description, decimal Price, Currency Currency);
