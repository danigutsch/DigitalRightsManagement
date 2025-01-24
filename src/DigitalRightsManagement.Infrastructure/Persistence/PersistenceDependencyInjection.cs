using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common;
using DigitalRightsManagement.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DigitalRightsManagement.Infrastructure.Persistence;

internal static class PersistenceDependencyInjection
{
    public static THostBuilder AddPersistence<THostBuilder>(this THostBuilder builder) where THostBuilder : IHostApplicationBuilder
    {
        // BUG: AddNpgsqlDbContext registers the DbContext as singleton, creating problems injecting domain event handler dependencies
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString(ResourceNames.Database));
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
        });

        builder.Services
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IProductRepository, ProductRepository>()
            .AddScoped<IResourceRepository, ResourceRepository>();

        return builder;
    }
}
