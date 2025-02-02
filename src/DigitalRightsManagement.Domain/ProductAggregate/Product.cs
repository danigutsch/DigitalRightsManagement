using Ardalis.Result;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Domain.AgentAggregate;
using DigitalRightsManagement.Domain.ProductAggregate.Events;

namespace DigitalRightsManagement.Domain.ProductAggregate;

public sealed partial class Product : AggregateRoot<ProductId>
{
    public ProductName Name { get; private set; }
    public Description Description { get; private set; }
    public Price Price { get; private set; }
    public AgentId AgentId { get; private init; }
    public ProductStatus Status { get; private set; } = ProductStatus.Development;

    private readonly List<AgentId> _assignedWorkers = [];
    public IReadOnlyList<AgentId> AssignedWorkers => _assignedWorkers.AsReadOnly();

    private Product(ProductName name, Description description, Price price, AgentId createdBy, ProductId? id = null) : base(id ?? ProductId.Create())
    {
        Name = name;
        Description = description;
        Price = price;
        AgentId = createdBy;

        QueueDomainEvent(new ProductCreated(Id, createdBy, name, description, price));
    }

    public static Result<Product> Create(ProductName name, Description description, Price price, AgentId manager, ProductId? id = null) => new Product(name, description, price, manager, id);

    public Result AssignWorker(AgentId userId, AgentId workerId)
    {
        var ownerValidation = ValidateOwner(userId);
        if (!ownerValidation.IsSuccess)
        {
            return ownerValidation;
        }

        if (_assignedWorkers.Contains(workerId))
        {
            return Errors.Products.Assignment.WorkerAlreadyAssigned(Id, workerId);
        }

        _assignedWorkers.Add(workerId);
        QueueDomainEvent(new WorkerAssigned(Id, userId, workerId));

        return Result.Success();
    }

    public Result UpdatePrice(AgentId agentId, Price newPrice, string reason)
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

    public Result UpdateDescription(AgentId agentId, Description newDescription)
    {
        var ownerValidation = ValidateOwner(agentId);
        if (!ownerValidation.IsSuccess)
        {
            return ownerValidation;
        }

        var oldDescription = Description;

        Description = newDescription;

        QueueDomainEvent(new DescriptionUpdated(Id, newDescription, oldDescription));

        return Result.Success();
    }

    public Result Publish(AgentId agentId)
    {
        var ownerValidation = ValidateOwner(agentId);
        if (!ownerValidation.IsSuccess)
        {
            return ownerValidation;
        }

        switch (Status)
        {
            case ProductStatus.Obsolete:
                return Errors.Products.Status.InvalidStatusChange(Status, ProductStatus.Published);
            case ProductStatus.Published:
                return Errors.Products.Status.AlreadyInStatus(Status);
        }

        Status = ProductStatus.Published;

        QueueDomainEvent(new ProductPublished(Id, agentId));

        return Result.Success();
    }

    public Result Obsolete(AgentId agentId)
    {
        var ownerValidation = ValidateOwner(agentId);
        if (!ownerValidation.IsSuccess)
        {
            return ownerValidation;
        }

        if (Status == ProductStatus.Obsolete)
        {
            return Errors.Products.Status.AlreadyInStatus(Status);
        }

        Status = ProductStatus.Obsolete;

        QueueDomainEvent(new ProductObsoleted(Id, agentId));

        return Result.Success();
    }

    private Result ValidateOwner(AgentId agentId)
    {
        if (agentId != AgentId)
        {
            return Errors.Products.Assignment.InvalidManager(agentId, AgentId);
        }

        return Result.Success();
    }

    public Result UnassignWorker(AgentId agentId, AgentId workerId)
    {
        var ownerValidation = ValidateOwner(agentId);
        if (!ownerValidation.IsSuccess)
        {
            return ownerValidation;
        }

        if (!_assignedWorkers.Contains(workerId))
        {
            return Errors.Products.Assignment.WorkerNotAssigned(Id, workerId);
        }

        _assignedWorkers.Remove(workerId);
        QueueDomainEvent(new WorkerUnassigned(Id, agentId, workerId));

        return Result.Success();
    }
}
