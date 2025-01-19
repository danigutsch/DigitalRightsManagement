using DigitalRightsManagement.Application;
using DigitalRightsManagement.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DigitalRightsManagement.Infrastructure.Identity;

public static class IdentityDependencyInjection
{
    public static TApplicationBuilder AddIdentity<TApplicationBuilder>(this TApplicationBuilder builder) where TApplicationBuilder : IHostApplicationBuilder
    {
        builder.AddNpgsqlDbContext<AuthDbContext>(ResourceNames.Database, settings => settings.DisableRetry = true);

        builder.Services.AddIdentityCore<AuthUser>()
            .AddRoles<IdentityRole>()
            .AddSignInManager()
            .AddEntityFrameworkStores<AuthDbContext>();

        builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

        return builder;
    }
}
