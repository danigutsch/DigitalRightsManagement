using Ardalis.Result;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Domain;
using DigitalRightsManagement.Domain.ProductAggregate;
using Microsoft.EntityFrameworkCore;

namespace DigitalRightsManagement.Infrastructure.Persistence;

internal sealed class ProductRepository(ApplicationDbContext context) : IProductRepository
{
    public IUnitOfWork UnitOfWork => context;

    public void Add(Product product) => context.Products.Add(product);
    public async Task<Result<Product>> GetById(Guid id, CancellationToken ct)
    {
        var product = await context.Products.FindAsync([id], ct);
        if (product is null)
        {
            return Errors.Product.NotFound(id);
        }

        return product;
    }

    public async Task<Result<IReadOnlyList<Product>>> GetById(IEnumerable<Guid> ids, CancellationToken ct) =>
        await context.Products
            .Where(p => ids.Contains(p.Id))
            .ToListAsync(ct);
}
