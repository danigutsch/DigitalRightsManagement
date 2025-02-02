using Ardalis.Result;

namespace DigitalRightsManagement.Domain.AgentAggregate;

public readonly record struct AgentId
{
    public Guid Value { get; }

    private AgentId(Guid value) => Value = value;

    public static AgentId Create() => new(Guid.CreateVersion7());

    public static AgentId From(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID cannot be empty.");
        }

        return new AgentId(id);
    }

    public static Result<AgentId> From(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return Errors.Agents.EmptyId();
        }

        if (!Guid.TryParse(id, out var guid))
        {
            return Errors.Agents.InvalidId(id);
        }

        return From(guid);
    }

    public override string ToString() => Value.ToString();
}
