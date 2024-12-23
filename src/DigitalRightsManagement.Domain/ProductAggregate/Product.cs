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
            return Errors.Product.InvalidName();
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return Errors.Product.InvalidDescription();
        }

        if (createdBy == Guid.Empty)
        {
            return Errors.Product.EmptyId();
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

    public Result Publish(Guid userId)
    {
        var result = ChangeStatus(userId, ProductStatus.Draft, ProductStatus.Published);
        if (!result.IsSuccess)
        {
            return result;
        }

        QueueDomainEvent(new ProductPublished(Id, userId));

        return Result.Success();
    }

    public Result Deprecate(Guid userId)
    {
        var result = ChangeStatus(userId, ProductStatus.Published, ProductStatus.Deprecated);
        if (!result.IsSuccess)
        {
            return result;
        }

        QueueDomainEvent(new ProductDeprecated(Id, userId));

        return Result.Success();
    }

    public Result MarkOutOfSupport(Guid userId)
    {
        var result = ChangeStatus(userId, ProductStatus.Deprecated, ProductStatus.OutOfSupport);
        if (!result.IsSuccess)
        {
            return result;
        }

        QueueDomainEvent(new ProductOutOfSupport(Id, userId));

        return Result.Success();
    }

    private Result ChangeStatus(Guid userId, ProductStatus validToProceed, ProductStatus desiredStatus)
    {
        if (userId == Guid.Empty)
        {
            return Errors.User.EmptyId();
        }

        if (Status != validToProceed)
        {
            return Errors.Product.InvalidStatusChange(Status, desiredStatus);
        }

        Status = desiredStatus;

        return Result.Success();
    }
}
