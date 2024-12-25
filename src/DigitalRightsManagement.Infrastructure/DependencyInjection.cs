using DigitalRightsManagement.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DigitalRightsManagement.Infrastructure;

public static class DependencyInjection
{
    public static THostBuilder AddInfrastructure<THostBuilder>(this THostBuilder builder) where THostBuilder : IHostApplicationBuilder
    {
        builder.AddNpgsqlDbContext<ApplicationDbContext>(PersistenceDefaults.ConnectionStringName);

        builder.Services
            .AddScoped<IUnitOfWork, ApplicationDbContext>()
            .AddScoped<IUserRepository, UserRepository>();

        return builder;
    }
}
