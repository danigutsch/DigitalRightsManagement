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

            applicationDbManager.SetSeedData(SeedData.Users, SeedData.Products);

            await applicationDbManager.EnsureDatabase(stoppingToken);
            await applicationDbManager.RunMigration(stoppingToken);
            await applicationDbManager.SeedDatabase(stoppingToken);

            var identityDbManager = scope.ServiceProvider.GetRequiredService<IIdentityDbManager>();

            identityDbManager.SetSeedData(SeedData.UsersAndPasswords);
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
