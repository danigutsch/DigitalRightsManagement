using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.Infrastructure.Authorization;
using DigitalRightsManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Transactions;

namespace DigitalRightsManagement.Infrastructure.Persistence.DbManagement;

public interface IIdentityDbManager : IDatabaseManager
{
    public void SetSeedData(IEnumerable<(User user, string password)> users);
}

internal sealed class IdentityDbManager(AuthDbContext dbContext, UserManager<AuthUser> userManager, RoleManager<IdentityRole> roleManager) : DatabaseManager<AuthDbContext>(dbContext), IIdentityDbManager
{
    private List<(User user, string password)> _users = [];

    public override async Task SeedDatabase(CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(_users.Count);

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await AddRoles();
        await AddUsers()
            .ConfigureAwait(false);

        scope.Complete();
    }

    private async Task AddUsers()
    {
        foreach (var (user, password) in _users)
        {
            var authUser = new AuthUser
            {
                DomainUserId = user.Id,
                UserName = user.Username,
                Email = user.Email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(authUser, password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to create user {authUser.UserName}");
            }

            var role = user.Role switch
            {
                UserRoles.Admin => AuthorizationRoles.Admin,
                UserRoles.Manager => AuthorizationRoles.Manager,
                UserRoles.Worker => AuthorizationRoles.Worker,
                _ => throw new InvalidOperationException($"Invalid role {user.Role}")
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

    public void SetSeedData(IEnumerable<(User user, string password)> users) => _users = [.. users];
}
