using Bogus;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.MigrationService.Factories;

public static class ProductFactory
{
    private static readonly Faker Faker = new();

    public static Product InDevelopment(
        string? name = null,
        string? description = null,
        Price? price = null,
        Guid? manager = null)
    {
        return Product.Create(
            name ?? Faker.Commerce.ProductName(),
            description ?? Faker.Commerce.ProductDescription(),
            price ?? Price.Create(Faker.Random.Decimal(1.00m, 100.00m), Faker.PickRandom<Currency>()).Value,
            manager ?? Faker.Random.Guid()
        ).Value;
    }

    public static Product Published(
        string? name = null,
        string? description = null,
        Price? price = null,
        Guid? manager = null)
    {
        var product = InDevelopment(name, description, price, manager);
        product.Publish(product.Manager);
        return product;
    }

    public static Product Obsolete(
        string? name = null,
        string? description = null,
        Price? price = null,
        Guid? manager = null)
    {
        var product = Published(name, description, price, manager);
        product.Obsolete(product.Manager);
        return product;
    }
}
