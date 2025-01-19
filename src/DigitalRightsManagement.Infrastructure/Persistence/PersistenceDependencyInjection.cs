using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DigitalRightsManagement.Infrastructure.Persistence;

internal static class PersistenceDependencyInjection
{
    public static THostBuilder AddPersistence<THostBuilder>(this THostBuilder builder) where THostBuilder : IHostApplicationBuilder
    {
        builder.AddNpgsqlDbContext<ApplicationDbContext>(ResourceNames.Database, settings => settings.DisableRetry = true);

        builder.Services
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IProductRepository, ProductRepository>();

        return builder;
    }
}
