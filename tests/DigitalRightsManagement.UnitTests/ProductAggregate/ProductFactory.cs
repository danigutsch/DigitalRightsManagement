using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.UnitTests.ProductAggregate;

internal static class ProductFactory
{
    public static Product InDevelopment(string name = "name", string description = "description", decimal price = 2.00m, Currency currency = Currency.Euro, Guid? createdBy = null)
    {
        return Product.Create(name, description, Price.Create(price, currency), createdBy ?? Guid.NewGuid());
    }

    public static Product Published(string name = "name", string description = "description", decimal price = 2.00m, Currency currency = Currency.Euro, Guid? createdBy = null)
    {
        var product = InDevelopment(name, description, price, currency, createdBy);
        product.Publish(product.CreatedBy);
        return product;
    }

    public static Product Obsolete(string name = "name", string description = "description", decimal price = 2.00m, Currency currency = Currency.Euro, Guid? createdBy = null)
    {
        var product = Published(name, description, price, currency, createdBy);
        product.Publish(product.CreatedBy);
        product.Obsolete(product.CreatedBy);
        return product;
    }
}
