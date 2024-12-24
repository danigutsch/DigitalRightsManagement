using Bogus;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.UnitTests.Common.Factories;

internal static class ProductFactory
{
    private static readonly Faker Faker = new();

    public static Product InDevelopment(
        string? name = null,
        string? description = null,
        Price? price = null,
        Guid? createdBy = null)
    {
        return Product.Create(
            name ?? Faker.Commerce.ProductName(),
            description ?? Faker.Commerce.ProductDescription(),
            price ?? Price.Create(Faker.Random.Decimal(1.00m, 100.00m), Faker.PickRandom<Currency>()).Value,
            createdBy ?? Faker.Random.Guid()
        ).Value;
    }

    public static Product Published(
        string? name = null,
        string? description = null,
        Price? price = null,
        Guid? createdBy = null)
    {
        var product = InDevelopment(name, description, price, createdBy);
        product.Publish(product.CreatedBy);
        return product;
    }

    public static Product Obsolete(
        string? name = null,
        string? description = null,
        Price? price = null,
        Guid? createdBy = null)
    {
        var product = Published(name, description, price, createdBy);
        product.Obsolete(product.CreatedBy);
        return product;
    }
}
