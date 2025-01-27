using Ardalis.Result;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.Messaging;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Application.ProductAggregate;

public sealed record GetProductsQuery : IQuery<ProductDto[]>
{
    internal sealed class GetProductsQueryHandler(ICurrentAgentProvider currentAgentProvider, IManagementQueries queries) : IQueryHandler<GetProductsQuery, ProductDto[]>
    {
        public async Task<Result<ProductDto[]>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            return await currentAgentProvider.Get(cancellationToken)
                .BindAsync(agent => queries.GetProductsByAgentId(agent.Id, cancellationToken))
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
