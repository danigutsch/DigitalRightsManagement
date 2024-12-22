using Ardalis.Result;
using DigitalRightsManagement.Common;

namespace DigitalRightsManagement.Domain.ProductAggregate;

public sealed class Product : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Price Price { get; private set; }

    private Product(string name, string description, Price price) : base(Guid.CreateVersion7())
    {
        Name = name;
        Description = description;
        Price = price;

        QueueDomainEvent(new ProductCreated(name, description, price));
    }

    public static Result<Product> Create(string name, string description, Price price)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Errors.Product.Create.InvalidName();
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return Errors.Product.Create.InvalidDescription();
        }

        var product = new Product(name, description, price);

        return product;
    }

    public void UpdatePrice(Price newPrice, string reason)
    {
        var oldPrice = Price;

        Price = newPrice;

        QueueDomainEvent(new PriceUpdated(Id, oldPrice, oldPrice, reason));
    }
}
