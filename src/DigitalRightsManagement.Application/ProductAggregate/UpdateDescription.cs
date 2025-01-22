using Ardalis.Result;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Common.Messaging;

namespace DigitalRightsManagement.Application.ProductAggregate;

public sealed record UpdateDescriptionCommand(Guid ProductId, string NewDescription) : ICommand
{
    internal sealed class UpdateDescriptionCommandHandler(ICurrentUserProvider currentUserProvider, IProductRepository productRepository) : ICommandHandler<UpdateDescriptionCommand>
    {
        public async Task<Result> Handle(UpdateDescriptionCommand command, CancellationToken cancellationToken)
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


            return await product.UpdateDescription(user.Id, command.NewDescription)
                .Tap(_ => productRepository.UnitOfWork.SaveEntities(cancellationToken))
                .MapAsync(_ => Result.Success());
        }
    }
}
