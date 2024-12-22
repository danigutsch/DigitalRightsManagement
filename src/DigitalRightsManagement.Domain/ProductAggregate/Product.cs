using Ardalis.Result;
using DigitalRightsManagement.Common;

namespace DigitalRightsManagement.Domain.ProductAggregate;

public sealed class Product : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Price Price { get; private set; }
    public Guid CreatedBy { get; private init; }
    public ProductStatus Status { get; private set; } = ProductStatus.Draft;

    private Product(string name, string description, Price price, Guid createdBy) : base(Guid.CreateVersion7())
    {
        Name = name;
        Description = description;
        Price = price;
        CreatedBy = createdBy;

        QueueDomainEvent(new ProductCreated(name, description, price));
    }

    public static Result<Product> Create(string name, string description, Price price, Guid createdBy)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Errors.Product.Create.InvalidName();
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return Errors.Product.Create.InvalidDescription();
        }

        if (createdBy == Guid.Empty)
        {
            return Errors.Product.Create.EmptyId();
        }

        var product = new Product(name, description, price, createdBy);

        return product;
    }

    public void UpdatePrice(Price newPrice, string reason)
    {
        var oldPrice = Price;

        Price = newPrice;

        QueueDomainEvent(new PriceUpdated(Id, newPrice, oldPrice, reason));
    }

    public void Publish(Guid userId)
    {
        if (Status != ProductStatus.Draft) throw new InvalidOperationException("Only draft products can be published.");

        Status = ProductStatus.Published;
        QueueDomainEvent(new ProductPublished(Id, userId));
    }

    public void Deprecate(Guid userId)
    {
        if (Status != ProductStatus.Published) throw new InvalidOperationException("Only published products can be deprecated.");

        Status = ProductStatus.Deprecated;
        QueueDomainEvent(new ProductDeprecated(Id, userId));
    }

    public void MarkOutOfSupport(Guid userId)
    {
        if (Status != ProductStatus.Deprecated) throw new InvalidOperationException("Only deprecated products can be marked out of support.");

        Status = ProductStatus.OutOfSupport;
        QueueDomainEvent(new ProductOutOfSupport(Id, userId));
    }

    public void Archive(Guid userId)
    {
        if (Status != ProductStatus.OutOfSupport && Status != ProductStatus.Published) throw new InvalidOperationException("Only out-of-support or published products can be archived.");

        Status = ProductStatus.Archived;
        QueueDomainEvent(new ProductArchived(Id, userId));
    }
}
