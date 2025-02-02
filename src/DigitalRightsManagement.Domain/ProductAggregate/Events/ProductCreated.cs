using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Domain.AgentAggregate;

namespace DigitalRightsManagement.Domain.ProductAggregate.Events;

public sealed record ProductCreated(ProductId ProductId, AgentId AgentId, ProductName Name, Description Description, Price Price) : DomainEvent;
