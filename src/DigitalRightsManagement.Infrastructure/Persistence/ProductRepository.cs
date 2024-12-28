using DigitalRightsManagement.Application;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Infrastructure.Persistence;

internal sealed class ProductRepository(ApplicationDbContext context) : IProductRepository
{
    public void Add(Product product) => context.Products.Add(product);
}
