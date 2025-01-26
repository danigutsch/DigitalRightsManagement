using DigitalRightsManagement.Common;
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
        builder.Services.AddDbContext<ManagementDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString(ResourceNames.Database));
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
        });

        return builder;
    }
}
