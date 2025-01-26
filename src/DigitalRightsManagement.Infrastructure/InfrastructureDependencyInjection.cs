﻿using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Infrastructure.Caching;
using DigitalRightsManagement.Infrastructure.Caching.Repositories;
using DigitalRightsManagement.Infrastructure.Identity;
using DigitalRightsManagement.Infrastructure.Messaging;
using DigitalRightsManagement.Infrastructure.Persistence;
using DigitalRightsManagement.Infrastructure.Persistence.DbManagement;
using DigitalRightsManagement.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DigitalRightsManagement.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static THostBuilder AddInfrastructure<THostBuilder>(this THostBuilder builder) where THostBuilder : IHostApplicationBuilder
    {
        builder.AddPersistence();

        builder.AddCaching();

        builder.Services.AddRepositories();

        builder.Services.AddMessaging();

        return builder;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // TODO: Add option to use cache or not
        services
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IProductRepository, ProductRepository>()
            .AddScoped<IResourceRepository>(provider =>
            {
                var dbContext = provider.GetRequiredService<ManagementDbContext>();
                var resourceRepository = new ResourceRepository(dbContext);
                var cache = provider.GetRequiredService<IDistributedCache>();

                return new CachedResourceRepository(resourceRepository, cache);
            });

        return services.AddScoped<IManagementQueries, ManagementQueries>();
    }

    public static THostBuilder AddMigrationInfrastructure<THostBuilder>(this THostBuilder builder) where THostBuilder : IHostApplicationBuilder
    {
        builder.AddNpgsqlDbContext<ManagementDbContext>(ResourceNames.Database, settings => settings.DisableRetry = true);
        builder.AddNpgsqlDbContext<AuthDbContext>(ResourceNames.Database, settings => settings.DisableRetry = true);

        builder.Services.AddScoped<IApplicationDbManager, ApplicationDbManager>();
        builder.Services.AddScoped<IIdentityDbManager, IdentityDbManager>();

        builder.Services.AddIdentityCore<AuthUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AuthDbContext>();

        // We only need the services for the ManagementDbContext
        builder.Services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblyContaining<AggregateRoot>());

        return builder;
    }

    public static THostBuilder AddIdentityInfrastructure<THostBuilder>(this THostBuilder builder)
        where THostBuilder : IHostApplicationBuilder => builder.AddIdentity();
}
