using DigitalRightsManagement.Application;
using DigitalRightsManagement.Common;
using DigitalRightsManagement.Infrastructure.Identity.Management;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DigitalRightsManagement.Infrastructure.Identity;

internal static class IdentityDependencyInjection
{
    public static TApplicationBuilder AddIdentity<TApplicationBuilder>(this TApplicationBuilder builder) where TApplicationBuilder : IHostApplicationBuilder
    {
        builder.AddNpgsqlDbContext<ManagementIdentityDbContext>(ResourceNames.Database, settings => settings.DisableRetry = true);

        builder.Services.AddIdentityCore<ManagementIdentityUser>()
            .AddRoles<IdentityRole>()
            .AddSignInManager()
            .AddEntityFrameworkStores<ManagementIdentityDbContext>();

        builder.Services.AddScoped<ICurrentAgentProvider, CurrentAgentProvider>();

        return builder;
    }
}
