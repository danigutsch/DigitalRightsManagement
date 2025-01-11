using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DigitalRightsManagement.Infrastructure.Persistence.Identity;

public static class IdentityDependencyInjection
{
    public static TApplicationBuilder AddBasicAuth<TApplicationBuilder>(this TApplicationBuilder builder) where TApplicationBuilder : IHostApplicationBuilder
    {
        builder.AddNpgsqlDbContext<AuthDbContext>(PersistenceDefaults.ConnectionStringName, settings => settings.DisableRetry = true);

        builder.Services.AddIdentityCore<AuthUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AuthDbContext>();

        return builder;
    }
}
