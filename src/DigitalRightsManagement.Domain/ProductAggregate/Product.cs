using Ardalis.Result;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Domain.ProductAggregate.Events;

namespace DigitalRightsManagement.Domain.ProductAggregate;

public sealed class Product : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Price Price { get; private set; }
    public Guid AgentId { get; private init; }
    public ProductStatus Status { get; private set; } = ProductStatus.Development;

    private readonly List<Guid> _assignedWorkers = [];
    public IReadOnlyList<Guid> AssignedWorkers => _assignedWorkers.AsReadOnly();

    private Product(string name, string description, Price price, Guid createdBy, Guid? id = null) : base(id ?? Guid.CreateVersion7())
    {
        Name = name.Trim();
        Description = description.Trim();
        Price = price;
        AgentId = createdBy;

        QueueDomainEvent(new ProductCreated(Id, createdBy, name, description, price));
    }

#pragma warning disable CS8618, CS9264
    private Product() { } // Do not use
#pragma warning restore CS8618, CS9264

    public static Result<Product> Create(string name, string description, Price price, Guid manager, Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Errors.Products.InvalidName();
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return Errors.Products.InvalidDescription();
        }

        if (manager == Guid.Empty)
        {
            return Errors.Products.EmptyCreatorId();
        }

        if (id is not null && id == Guid.Empty)
        {
            return Errors.Products.EmptyId();
        }

        var product = new Product(name, description, price, manager, id);

        return product;
    }

    public Result AssignWorker(Guid userId, Guid workerId)
    {
        var ownerValidation = ValidateOwner(userId);
        if (!ownerValidation.IsSuccess)
        {
            return ownerValidation;
        }

        if (workerId == Guid.Empty)
        {
            return Errors.Agents.EmptyId();
        }

        if (_assignedWorkers.Contains(workerId))
        {
            return Errors.Products.WorkerAlreadyAssigned(Id, workerId);
        }

        _assignedWorkers.Add(workerId);
        QueueDomainEvent(new WorkerAssigned(Id, userId, workerId));

        return Result.Success();
    }

    public Result UpdatePrice(Guid agentId, Price newPrice, string reason)
    {
        var ownerValidation = ValidateOwner(agentId);
        if (!ownerValidation.IsSuccess)
        {
            return ownerValidation;
        }

        var oldPrice = Price;

        Price = newPrice;

        QueueDomainEvent(new PriceUpdated(Id, newPrice, oldPrice, reason));

        return Result.Success();
    }

    public Result UpdateDescription(Guid agentId, string newDescription)
    {
        var ownerValidation = ValidateOwner(agentId);
        if (!ownerValidation.IsSuccess)
        {
            return ownerValidation;
        }

        if (string.IsNullOrWhiteSpace(newDescription))
        {
            return Errors.Products.InvalidDescription();
        }

        var oldDescription = Description;

        Description = newDescription;

        QueueDomainEvent(new DescriptionUpdated(Id, newDescription, oldDescription));

        return Result.Success();
    }

    public Result Publish(Guid agentId)
    {
        var ownerValidation = ValidateOwner(agentId);
        if (!ownerValidation.IsSuccess)
        {
            return ownerValidation;
        }

        switch (Status)
        {
            case ProductStatus.Obsolete:
                return Errors.Products.InvalidStatusChange(Status, ProductStatus.Published);
            case ProductStatus.Published:
                return Errors.Products.AlreadyInStatus(Status);
        }

        Status = ProductStatus.Published;

        QueueDomainEvent(new ProductPublished(Id, agentId));

        return Result.Success();
    }

    public Result Obsolete(Guid agentId)
    {
        var ownerValidation = ValidateOwner(agentId);
        if (!ownerValidation.IsSuccess)
        {
            return ownerValidation;
        }

        if (Status == ProductStatus.Obsolete)
        {
            return Errors.Products.AlreadyInStatus(Status);
        }

        Status = ProductStatus.Obsolete;

        QueueDomainEvent(new ProductObsoleted(Id, agentId));

        return Result.Success();
    }

    private Result ValidateOwner(Guid agentId)
    {
        if (agentId == Guid.Empty)
        {
            return Errors.Agents.EmptyId();
        }

        if (agentId != AgentId)
        {
            return Errors.Products.InvalidManager(agentId, AgentId);
        }

        return Result.Success();
    }

    public Result UnassignWorker(Guid userId, Guid workerId)
    {
        var ownerValidation = ValidateOwner(userId);
        if (!ownerValidation.IsSuccess)
        {
            return ownerValidation;
        }

        if (workerId == Guid.Empty)
        {
            return Errors.Agents.EmptyId();
        }

        if (!_assignedWorkers.Contains(workerId))
        {
            return Errors.Products.WorkerNotAssigned(Id, workerId);
        }

        _assignedWorkers.Remove(workerId);
        QueueDomainEvent(new WorkerUnassigned(Id, userId, workerId));

        return Result.Success();
    }
}
