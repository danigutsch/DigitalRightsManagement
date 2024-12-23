using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.UnitTests;

internal static class ProductFactory
{
    public static Product CreateProduct(string name = "name", string description = "description", decimal price = 2.00m, Currency currency = Currency.Euro, Guid? createdBy = null)
    {
        return Product.Create(name, description, Price.Create(price, currency), createdBy ?? Guid.NewGuid());
    }
}
