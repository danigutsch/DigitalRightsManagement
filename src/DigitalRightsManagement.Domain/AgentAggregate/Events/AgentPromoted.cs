using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.Domain.AgentAggregate.Events;

public sealed record AgentPromoted(AgentId AdminId, AgentId ManagerId, AgentRoles OldRole, AgentRoles NewRole) : DomainEvent;
