using DigitalRightsManagement.Domain.AgentAggregate;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalRightsManagement.Infrastructure.Persistence.Converters;

internal sealed class AgentIdListConverter()
    : ValueConverter<List<AgentId>, IEnumerable<Guid>>(
        ids => ids.Select(id => id.Value),
        ids => ids.Select(AgentId.From).ToList());

internal sealed class AgentIdConverter()
    : ValueConverter<AgentId, Guid>(
        id => id.Value,
        id => AgentId.From(id));
