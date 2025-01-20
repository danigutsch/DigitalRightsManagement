using Ardalis.Result;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Application.ProductAggregate;
using DigitalRightsManagement.Common.Messaging;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Application.UserAggregate;

public sealed class GetProductsQueryHandler(ICurrentUserProvider currentUserProvider, IProductRepository productRepository) : IQueryHandler<GetProductsQuery, ProductDto[]>
{
    public async Task<Result<ProductDto[]>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        return await currentUserProvider.Get(cancellationToken)
            .BindAsync(user => productRepository.GetById(user.Products, cancellationToken))
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

public sealed record GetProductsQuery : IQuery<ProductDto[]>;
