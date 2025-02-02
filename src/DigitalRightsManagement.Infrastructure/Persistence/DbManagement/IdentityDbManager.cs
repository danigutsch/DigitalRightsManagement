using DigitalRightsManagement.Infrastructure.Authorization;
using DigitalRightsManagement.Infrastructure.Identity.Management;
using Microsoft.AspNetCore.Identity;
using System.Transactions;
using DigitalRightsManagement.Domain.AgentAggregate;

namespace DigitalRightsManagement.Infrastructure.Persistence.DbManagement;

public interface IIdentityDbManager : IDatabaseManager
{
    public void SetSeedData(IEnumerable<(Agent agent, string password)> users);
}

internal sealed class IdentityDbManager(ManagementIdentityDbContext dbContext, UserManager<ManagementIdentityUser> userManager, RoleManager<IdentityRole> roleManager) : DatabaseManager<ManagementIdentityDbContext>(dbContext), IIdentityDbManager
{
    private List<(Agent agent, string password)> _agents = [];

    public override async Task SeedDatabase(CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(_agents.Count);

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await AddRoles();
        await AddUsers()
            .ConfigureAwait(false);

        scope.Complete();
    }

    private async Task AddUsers()
    {
        foreach (var (agent, password) in _agents)
        {
            var authUser = new ManagementIdentityUser
            {
                DomainUserId = agent.Id.Value,
                UserName = agent.Username.Value,
                Email = agent.Email.Value,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(authUser, password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to create user {authUser.UserName}");
            }

            var role = agent.Role switch
            {
                AgentRoles.Admin => AuthorizationRoles.Admin,
                AgentRoles.Manager => AuthorizationRoles.Manager,
                AgentRoles.Worker => AuthorizationRoles.Worker,
                _ => throw new InvalidOperationException($"Invalid role {agent.Role}")
            };

            await userManager.AddToRoleAsync(authUser, role);
        }
    }

    private async Task AddRoles()
    {
        var roles = new[] { AuthorizationRoles.Worker, AuthorizationRoles.Manager, AuthorizationRoles.Admin }
            .Select(r => new IdentityRole(r));

        foreach (var role in roles)
        {
            var result = await roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to create role {role.Name}");
            }
        }
    }

    public void SetSeedData(IEnumerable<(Agent agent, string password)> users) => _agents = [.. users];
}
