using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.Domain.AgentAggregate.Events;

public sealed record AgentPromoted(Guid AdminId, Guid ManagerId, AgentRoles OldRole, AgentRoles NewRole) : DomainEvent;
