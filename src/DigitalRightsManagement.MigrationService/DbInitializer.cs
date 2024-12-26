using DigitalRightsManagement.Infrastructure.Persistence.Migrations;
using System.Diagnostics;

namespace DigitalRightsManagement.MigrationService;

internal sealed class DbInitializer(
    IServiceProvider serviceProvider,
    IHostEnvironment hostEnvironment,
    IHostApplicationLifetime hostApplicationLifetime)
    : BackgroundService
{
    private readonly ActivitySource _activitySource = new(hostEnvironment.ApplicationName);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = _activitySource.StartActivity(hostEnvironment.ApplicationName, ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var databaseManager = scope.ServiceProvider.GetRequiredService<IDatabaseManager>();
            await databaseManager.EnsureDatabase(stoppingToken);
            await databaseManager.InitializeDatabase(SeedData.GetUsers(), stoppingToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    public override void Dispose()
    {
        _activitySource.Dispose();

        base.Dispose();
    }
}
