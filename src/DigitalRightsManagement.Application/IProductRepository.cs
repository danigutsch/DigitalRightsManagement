using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Application;

public interface IProductRepository
{
    void Add(Product product);
}
