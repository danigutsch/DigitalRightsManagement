using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DigitalRightsManagement.Application;

public static class DependencyInjection
{
    public static THostBuilder AddApplication<THostBuilder>(this THostBuilder builder) where THostBuilder : IHostApplicationBuilder
    {
        builder.Services.AddScoped<ChangeUserRoleCommandHandler>();
        builder.Services.AddScoped<ChangeEmailCommandHandler>();

        return builder;
    }
}
