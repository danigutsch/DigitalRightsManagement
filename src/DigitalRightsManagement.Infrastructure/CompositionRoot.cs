using DigitalRightsManagement.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DigitalRightsManagement.Infrastructure;

public static class CompositionRoot
{
    public static THostApplicationBuilder AddInfrastructure<THostApplicationBuilder>(this THostApplicationBuilder builder) where THostApplicationBuilder : IHostApplicationBuilder
    {
        builder.AddNpgsqlDbContext<ApplicationDbContext>(PersistenceDefaults.ConnectionStringName);

        builder.Services
            .AddScoped<IUnitOfWork, ApplicationDbContext>()
            .AddScoped<IUserRepository, UserRepository>();

        return builder;
    }
}
