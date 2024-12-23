using Bogus;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.UnitTests.ProductAggregate;

internal static class ProductFactory
{
    private static readonly Faker<Product> Faker = new Faker<Product>()
        .CustomInstantiator(f => Product.Create(
            f.Commerce.ProductName(),
            f.Commerce.ProductDescription(),
            Price.Create(f.Random.Decimal(1.00m, 100.00m), f.PickRandom<Currency>()),
            f.Random.Guid()
            ).Value
        );

    public static Product InDevelopment() => Faker.Generate();

    public static Product Published()
    {
        var product = InDevelopment();
        product.Publish(product.CreatedBy);
        return product;
    }

    public static Product Obsolete()
    {
        var product = Published();
        product.Publish(product.CreatedBy);
        product.Obsolete(product.CreatedBy);
        return product;
    }
}
