using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common;
using DigitalRightsManagement.Infrastructure.Identity;
using DigitalRightsManagement.Infrastructure.Messaging;
using DigitalRightsManagement.Infrastructure.Persistence;
using DigitalRightsManagement.Infrastructure.Persistence.DbManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DigitalRightsManagement.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static THostBuilder AddInfrastructure<THostBuilder>(this THostBuilder builder) where THostBuilder : IHostApplicationBuilder
    {
        builder.AddNpgsqlDbContext<ApplicationDbContext>(ResourceNames.Database, settings => settings.DisableRetry = true);

        builder.Services
            .AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>())
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IProductRepository, ProductRepository>();

        builder.Services.AddMessaging();

        return builder;
    }

    public static THostBuilder AddMigrationInfrastructure<THostBuilder>(this THostBuilder builder) where THostBuilder : IHostApplicationBuilder
    {
        builder.AddNpgsqlDbContext<ApplicationDbContext>(ResourceNames.Database, settings => settings.DisableRetry = true);
        builder.AddNpgsqlDbContext<AuthDbContext>(ResourceNames.Database, settings => settings.DisableRetry = true);

        builder.Services.AddScoped<IApplicationDbManager, ApplicationDbManager>();
        builder.Services.AddScoped<IIdentityDbManager, IdentityDbManager>();

        builder.Services.AddIdentityCore<AuthUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AuthDbContext>();

        // We only need the services for the ApplicationDbContext
        builder.Services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(ResourceNames).Assembly));

        return builder;
    }

    public static THostBuilder AddIdentityInfrastructure<THostBuilder>(this THostBuilder builder)
        where THostBuilder : IHostApplicationBuilder => builder.AddIdentity();
}
