using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Application.Persistence;

public interface IProductRepository
{
    void Add(Product product);
}
