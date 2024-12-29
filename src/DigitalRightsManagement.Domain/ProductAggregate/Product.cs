using Ardalis.Result;
using DigitalRightsManagement.Common;
using DigitalRightsManagement.Domain.ProductAggregate.Events;

namespace DigitalRightsManagement.Domain.ProductAggregate;

public sealed class Product : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Price Price { get; private set; }
    public Guid Manager { get; private init; }
    public ProductStatus Status { get; private set; } = ProductStatus.Development;

    private Product(string name, string description, Price price, Guid createdBy) : base(Guid.CreateVersion7())
    {
        Name = name.Trim();
        Description = description.Trim();
        Price = price;
        Manager = createdBy;

        QueueDomainEvent(new ProductCreated(name, description, price));
    }

#pragma warning disable CS8618, CS9264
    private Product() { } // Do not use
#pragma warning restore CS8618, CS9264

    public static Result<Product> Create(string name, string description, Price price, Guid manager)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Errors.Product.InvalidName();
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return Errors.Product.InvalidDescription();
        }

        if (manager == Guid.Empty)
        {
            return Errors.Product.EmptyId();
        }

        var product = new Product(name, description, price, manager);

        return product;
    }

    public Result UpdatePrice(Guid userId, Price newPrice, string reason)
    {
        var ownerValidation = ValidateOwner(userId);
        if (!ownerValidation.IsSuccess)
        {
            return ownerValidation;
        }

        var oldPrice = Price;

        Price = newPrice;

        QueueDomainEvent(new PriceUpdated(Id, newPrice, oldPrice, reason));

        return Result.Success();
    }

    public Result UpdateDescription(Guid userId, string newDescription)
    {
        var ownerValidation = ValidateOwner(userId);
        if (!ownerValidation.IsSuccess)
        {
            return ownerValidation;
        }

        if (string.IsNullOrWhiteSpace(newDescription))
        {
            return Errors.Product.InvalidDescription();
        }

        var oldDescription = Description;

        Description = newDescription;

        QueueDomainEvent(new DescriptionUpdated(Id, newDescription, oldDescription));

        return Result.Success();
    }

    public Result Publish(Guid userId)
    {
        var ownerValidation = ValidateOwner(userId);
        if (!ownerValidation.IsSuccess)
        {
            return ownerValidation;
        }

        switch (Status)
        {
            case ProductStatus.Obsolete:
                return Errors.Product.InvalidStatusChange(Status, ProductStatus.Published);
            case ProductStatus.Published:
                return Errors.Product.AlreadyInStatus(Status);
        }

        Status = ProductStatus.Published;

        QueueDomainEvent(new ProductPublished(Id, userId));

        return Result.Success();
    }

    public Result Obsolete(Guid userId)
    {
        var ownerValidation = ValidateOwner(userId);
        if (!ownerValidation.IsSuccess)
        {
            return ownerValidation;
        }

        if (Status == ProductStatus.Obsolete)
        {
            return Errors.Product.AlreadyInStatus(Status);
        }

        Status = ProductStatus.Obsolete;

        QueueDomainEvent(new ProductObsoleted(Id, userId));

        return Result.Success();
    }

    private Result ValidateOwner(Guid userId)
    {   
        if (userId == Guid.Empty)
        {
            return Errors.User.EmptyId();
        }

        if (userId != Manager)
        {
            return Errors.Product.InvalidManager(userId, Manager);
        }

        return Result.Success();
    }
}
