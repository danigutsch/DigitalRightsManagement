using DigitalRightsManagement.Domain.AgentAggregate;

namespace DigitalRightsManagement.Application.AgentAggregate;

public sealed record AgentDto(Guid Id, string Username, string Email, AgentRoles Role, IReadOnlyList<Guid> Products);
