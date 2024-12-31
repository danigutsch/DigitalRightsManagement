using Ardalis.Result;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Application.ProductAggregate;
using DigitalRightsManagement.Common.Messaging;

namespace DigitalRightsManagement.Application.UserAggregate;

public sealed class GetProductsQueryHandler(IUserRepository userRepository, IProductRepository productRepository) : IQueryHandler<GetProductsQuery, Result<ProductDto[]>>
{
    public async Task<Result<ProductDto[]>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        return await userRepository.GetById(request.UserId, cancellationToken)
            .BindAsync(user => productRepository.GetById(user.Products, cancellationToken))
            .MapAsync(products =>
                products.Select(product =>
                        new ProductDto(
                            product.Name,
                            product.Description,
                            product.Price.Amount,
                            product.Price.Currency)
                    )
                    .ToArray()
                );
    }
}

public sealed record GetProductsQuery(Guid UserId) : IQuery<Result<ProductDto[]>>;
