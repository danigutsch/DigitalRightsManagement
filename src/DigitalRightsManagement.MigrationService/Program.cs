using DigitalRightsManagement.Infrastructure;
using DigitalRightsManagement.MigrationService;
using DigitalRightsManagement.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Logging.SetMinimumLevel(LogLevel.Warning);

builder.AddMigrationInfrastructure();

builder.Services.AddHostedService<DbInitializer>();

var app = builder.Build();

await app.RunAsync();
