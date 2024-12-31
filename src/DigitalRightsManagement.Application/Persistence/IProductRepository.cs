using Ardalis.Result;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Application.Persistence;

public interface IProductRepository
{
    void Add(Product product);
    Task<Result<IReadOnlyList<Product>>> GetById(IEnumerable<Guid> ids, CancellationToken ct);
}
