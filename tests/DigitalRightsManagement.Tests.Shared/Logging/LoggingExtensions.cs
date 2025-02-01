using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace DigitalRightsManagement.Tests.Shared.Logging;

public static class LoggingExtensions
{
    public static WebApplicationFactory<TStartup> WithTestLogging<TStartup>(this WebApplicationFactory<TStartup> factory, ITestOutputHelper output) where TStartup : class
    {
        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();

                logging.Services.AddSingleton<ILoggerProvider>(_ => new XUnitLoggerProvider(output));
            });
        });
    }
}
