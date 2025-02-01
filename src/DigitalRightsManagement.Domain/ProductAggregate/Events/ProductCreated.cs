using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.Domain.ProductAggregate.Events;

public sealed record ProductCreated(Guid ProductId, Guid AgentId, ProductName Name, string Description, Price Price) : DomainEvent;
