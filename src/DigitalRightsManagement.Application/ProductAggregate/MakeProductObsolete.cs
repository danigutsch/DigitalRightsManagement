using Ardalis.Result;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Common.Messaging;

namespace DigitalRightsManagement.Application.ProductAggregate;

internal sealed class MakeProductObsoleteCommandHandler(ICurrentUserProvider currentUserProvider, IProductRepository productRepository) : ICommandHandler<MakeProductObsoleteCommand>
{
    public async Task<Result> Handle(MakeProductObsoleteCommand command, CancellationToken cancellationToken)
    {
        var userResult = await currentUserProvider.Get(cancellationToken);
        if (!userResult.IsSuccess)
        {
            return userResult.Map();
        }

        var user = userResult.Value;

        return await productRepository.GetById(command.ProductId, cancellationToken)
            .BindAsync(product => product.Obsolete(user.Id))
            .Tap(() => productRepository.UnitOfWork.SaveEntities(cancellationToken));
    }
}

public sealed record MakeProductObsoleteCommand(Guid ProductId) : ICommand;
