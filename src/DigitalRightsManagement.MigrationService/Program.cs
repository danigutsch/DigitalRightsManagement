using DigitalRightsManagement.MigrationService;
using DigitalRightsManagement.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<DbInitializer>();
