﻿using Bogus;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Tests.Shared.Factories;

public static class ProductFactory
{
    private static readonly Faker<Product> Faker = new Faker<Product>()
        .CustomInstantiator(f => Product.Create(
            ProductName.From(f.Commerce.ProductName()),
            f.Commerce.ProductDescription(),
            Price.Create(
                f.Random.Decimal(0, 100),
                f.PickRandom<Currency>()),
            Guid.NewGuid(),
            Guid.NewGuid()));

    public static Product InDevelopment(
        ProductName? name = null,
        string? description = null,
        Price? price = null,
        Guid? manager = null,
        Guid? id = null)
    {
        var product = Faker.Generate();

        return Product.Create(
            name ?? product.Name,
            description ?? product.Description,
            price ?? product.Price,
            manager ?? product.AgentId,
            id
        ).Value;
    }

    public static Product Published(
        ProductName? name = null,
        string? description = null,
        Price? price = null,
        Guid? manager = null,
        Guid? id = null)
    {
        var product = InDevelopment(name, description, price, manager, id);
        product.Publish(product.AgentId);
        return product;
    }

    public static Product Obsolete(
        ProductName? name = null,
        string? description = null,
        Price? price = null,
        Guid? manager = null,
        Guid? id = null)
    {
        var product = Published(name, description, price, manager, id);
        product.Obsolete(product.AgentId);
        return product;
    }
}
