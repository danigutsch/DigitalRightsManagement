﻿using DigitalRightsManagement.Common;
using DigitalRightsManagement.Infrastructure.Persistence;
using DigitalRightsManagement.Infrastructure.Persistence.DbManagement;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace DigitalRightsManagement.IntegrationTests;

public sealed class ApiFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly IHost _app;

    private string _dbConnectionString = null!;

    private IResourceBuilder<PostgresServerResource> DatabaseServer { get; }
    private IResourceBuilder<PostgresDatabaseResource> Database { get; }

    public ApiFixture()
    {
        var options = new DistributedApplicationOptions { AssemblyName = typeof(ApiFixture).Assembly.FullName, DisableDashboard = true };
        var appBuilder = DistributedApplication.CreateBuilder(options);

        DatabaseServer = appBuilder.AddPostgres(ResourceNames.DatabaseServer);
        Database = DatabaseServer.AddDatabase(ResourceNames.Database);

        appBuilder.AddProject<Projects.DigitalRightsManagement_MigrationService>(ResourceNames.MigrationService)
            .WithReference(Database)
            .WaitFor(Database);

        _app = appBuilder.Build();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"ConnectionStrings:{ResourceNames.Database}"] = _dbConnectionString
            });
        });

        builder.ConfigureServices(services =>
        {
            services
                .AddScoped<IApplicationDbManager, ApplicationDbManager>()
                .AddScoped<IIdentityDbManager, IdentityDbManager>();
        });

        return base.CreateHost(builder);
    }

    public async Task InitializeAsync()
    {
        var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();

        await _app.StartAsync();

        await resourceNotificationService.WaitForResourceAsync(ResourceNames.MigrationService, KnownResourceStates.Finished).WaitAsync(TimeSpan.FromSeconds(60));

        var dbConnectionString = await DatabaseServer.Resource.GetConnectionStringAsync() ?? throw new InvalidOperationException("Empty database connection string");
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(dbConnectionString)
        {
            Database = ResourceNames.Database
        };

        _dbConnectionString = connectionStringBuilder.ConnectionString;
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _app.StopAsync();
        if (_app is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync().ConfigureAwait(false);
        }
        else
        {
            _app.Dispose();
        }
    }
}
