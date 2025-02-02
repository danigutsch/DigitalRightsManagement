using Bogus;
using DigitalRightsManagement.Domain.AgentAggregate;
using DigitalRightsManagement.MigrationService;

namespace DigitalRightsManagement.Tests.Shared.Factories;

public static class AgentFactory
{
    private static readonly Faker<Agent> Faker = new Faker<Agent>()
        .CustomInstantiator(f => Agent.Create(
            f.Person.UserName,
            EmailAddress.From(f.Internet.Email()),
            f.PickRandom<AgentRoles>()));

    public static Agent Create(
        string? username = null,
        EmailAddress? email = null,
        AgentRoles? role = null,
        AgentId? id = null)
    {
        var agent = Faker.Generate();

        return Agent.Create(
            username ?? agent.Username,
            email ?? agent.Email,
            role ?? agent.Role,
            id);
    }

    public static Agent Seeded(AgentRoles? role = null)
    {
        IEnumerable<Agent> filtered = SeedData.Agents;

        if (role is not null)
        {
            filtered = filtered.Where(u => u.Role == role);
        }

        return filtered.Random();
    }

    public static Agent Seeded(Func<Agent, bool> func) => SeedData.Agents.Where(func).Random();
}
