using Ardalis.Result;
using DigitalRightsManagement.Application.Authorization;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.Messaging;
using DigitalRightsManagement.Domain.ProductAggregate;
using DigitalRightsManagement.Domain.UserAggregate;

namespace DigitalRightsManagement.Application.ProductAggregate;

[Authorize(UserRoles.Manager)]
public sealed record GetProductsQuery : IQuery<ProductDto[]>
{
    internal sealed class GetProductsQueryHandler(ICurrentUserProvider currentUserProvider, IManagementQueries queries) : IQueryHandler<GetProductsQuery, ProductDto[]>
    {
        public async Task<Result<ProductDto[]>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            return await currentUserProvider.Get(cancellationToken)
                .BindAsync(user => queries.GetProductsByUserId(user.Id, cancellationToken))
                .MapAsync(MapToDto);
        }

        private static ProductDto[] MapToDto(IReadOnlyList<Product> products) =>
        [.. products.Select(product =>
            new ProductDto(
                product.Name,
                product.Description,
                product.Price.Amount,
                product.Price.Currency)
        )];
    }
}
