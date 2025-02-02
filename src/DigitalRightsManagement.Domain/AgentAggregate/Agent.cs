using Ardalis.Result;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Domain.AgentAggregate.Events;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Domain.AgentAggregate;

public sealed partial class Agent : AggregateRoot<AgentId>
{
    public string Username { get; private set; }
    public string Email { get; private set; }
    public AgentRoles Role { get; private set; }

    private readonly List<ProductId> _products = [];
    public IReadOnlyList<ProductId> Products => _products.AsReadOnly();

    private Agent(string username, string email, AgentRoles role, AgentId? id = null) : base(id ?? AgentId.Create())
    {
        Username = username.Trim();
        Email = email.Trim();
        Role = role;

        QueueDomainEvent(new AgentCreated(Id, username, email, role));
    }

    public static Result<Agent> Create(string username, string email, AgentRoles role, AgentId? id = null)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return Errors.Agents.InvalidUsername();
        }

        var emailValidation = ValidateEmail(email);
        if (!emailValidation.IsSuccess)
        {
            return emailValidation;
        }

        if (!Enum.IsDefined(role))
        {
            return Errors.Agents.UnknownRole();
        }

        var agent = new Agent(username, email, role, id);

        return agent;
    }

    public Result ChangeRole(Agent admin, AgentRoles newRole)
    {
        if (!Enum.IsDefined(newRole))
        {
            return Errors.Agents.UnknownRole();
        }

        if (Role == newRole)
        {
            return Errors.Agents.AlreadyInRole(Id, newRole);
        }

        if (admin.Role != AgentRoles.Admin)
        {
            return Errors.Agents.UnauthorizedToPromote(admin.Id, Id, newRole);
        }

        Role = newRole;

        QueueDomainEvent(new AgentPromoted(admin.Id, Id, Role, newRole));

        return Result.Success();
    }

    public Result AddProduct(ProductId productId)
    {
        if (Role != AgentRoles.Manager && Role != AgentRoles.Worker)
        {
            return Errors.Agents.UnauthorizedToOwnProduct(Id);
        }

        if (Products.Contains(productId))
        {
            return Errors.Products.AlreadyAssigned(Id, productId);
        }

        _products.Add(productId);

        QueueDomainEvent(new ProductAdded(Id, productId));

        return Result.Success();
    }

    public Result AddProducts(IEnumerable<ProductId> productsIds)
    {
        if (Role != AgentRoles.Manager && Role != AgentRoles.Worker)
        {
            return Errors.Agents.UnauthorizedToOwnProduct(Id);
        }

        ProductId[] newProductIds = [..productsIds.Except(Products)];

        if (newProductIds.Length == 0)
        {
            return Errors.Products.AlreadyAssigned(Id);
        }

        _products.AddRange([.. newProductIds]);

        foreach (var productId in newProductIds)
        {
            QueueDomainEvent(new ProductAdded(Id, productId));
        }

        return Result.Success();
    }

    public Result RemoveProduct(ProductId productId)
    {
        if (!_products.Contains(productId))
        {
            return Result.Success();
        }

        _products.Remove(productId);
        QueueDomainEvent(new ProductRemoved(Id, productId));

        return Result.Success();
    }

    public Result ChangeEmail(string newEmail)
    {
        var emailValidation = ValidateEmail(newEmail);
        if (!emailValidation.IsSuccess)
        {
            return emailValidation;
        }

        Email = newEmail.Trim();

        QueueDomainEvent(new EmailUpdated(Id, newEmail));

        return Result.Success();
    }

    private static Result ValidateEmail(string email)
    {
        var trimmedEmail = email.Trim();

        if (trimmedEmail.AsSpan().ContainsAny([' ', '\n', '\r', '\t']))
        {
            return Errors.Agents.InvalidEmail();
        }

        var indexOfAt = trimmedEmail.IndexOf('@', StringComparison.Ordinal);
        if (indexOfAt <= 0)
        {
            return Errors.Agents.InvalidEmail();
        }

        if (indexOfAt != trimmedEmail.LastIndexOf('@'))
        {
            return Errors.Agents.InvalidEmail();
        }

        var indexOfDot = trimmedEmail.LastIndexOf('.');
        if (indexOfDot <= 0)
        {
            return Errors.Agents.InvalidEmail();
        }

        if (indexOfDot <= indexOfAt + 2)
        {
            return Errors.Agents.InvalidEmail();
        }

        if (indexOfDot == trimmedEmail.Length - 1)
        {
            return Errors.Agents.InvalidEmail();
        }

        return Result.Success();
    }
}
