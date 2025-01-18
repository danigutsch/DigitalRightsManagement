using DigitalRightsManagement.Application.Messaging;
using Microsoft.Extensions.Hosting;

namespace DigitalRightsManagement.Application;

public static class ApplicationDependencyInjection
{
    public static THostBuilder AddApplication<THostBuilder>(this THostBuilder builder) where THostBuilder : IHostApplicationBuilder
    {
        builder.Services.AddMessaging();

        return builder;
    }
}
