using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.Domain.AgentAggregate.Events;

public sealed record AgentCreated(AgentId Id, string Username, EmailAddress Email, AgentRoles Role) : DomainEvent;
