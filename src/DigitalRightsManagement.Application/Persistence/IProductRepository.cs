using Ardalis.Result;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Application.Persistence;

public interface IProductRepository : IRepository<Product>
{
    void Add(Product product);
    Task<Result<Product>> GetById(ProductId id, CancellationToken ct);
    Task<Result<IReadOnlyList<Product>>> GetById(IEnumerable<ProductId> ids, CancellationToken ct);
}
