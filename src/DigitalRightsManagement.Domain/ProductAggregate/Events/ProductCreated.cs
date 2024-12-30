using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.Domain.ProductAggregate.Events;

public sealed record ProductCreated(string Name, string Description, Price Price) : DomainEvent;
