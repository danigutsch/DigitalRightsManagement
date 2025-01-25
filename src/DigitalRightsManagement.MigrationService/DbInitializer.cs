using DigitalRightsManagement.Infrastructure.Persistence.DbManagement;
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
            var applicationDbManager = scope.ServiceProvider.GetRequiredService<IApplicationDbManager>();
            var identityDbManager = scope.ServiceProvider.GetRequiredService<IIdentityDbManager>();

            applicationDbManager.SetSeedData(SeedData.Users, SeedData.Products);
            identityDbManager.SetSeedData(SeedData.UsersAndPasswords);

            await CreateDatabases(stoppingToken, applicationDbManager, identityDbManager);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task CreateDatabases(CancellationToken stoppingToken, params IDatabaseManager[] databaseManagers)
    {
        foreach (var databaseManager in databaseManagers)
        {
            await databaseManager.EnsureDatabase(stoppingToken);
            await databaseManager.RunMigration(stoppingToken);
            await databaseManager.SeedDatabase(stoppingToken);
        }
    }

    public override void Dispose()
    {
        _activitySource.Dispose();

        base.Dispose();
    }
}
