using DigitalRightsManagement.Common;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis(ResourceNames.Cache)
    .WithRedisCommander()
    .WithRedisInsight();

var databaseServer = builder.AddPostgres(ResourceNames.DatabaseServer)
    .WithPgWeb();

var database = databaseServer.AddDatabase(ResourceNames.Database);

var dbInitializer = builder.AddProject<Projects.DigitalRightsManagement_MigrationService>(ResourceNames.MigrationService)
    .WithReference(database)
    .WaitFor(database);

builder.AddProject<Projects.DigitalRightsManagement_Api>(ResourceNames.Api)
    .WithReference(database)
    .WaitFor(database)
    .WithReference(cache)
    .WaitFor(cache)
    .WaitForCompletion(dbInitializer);

await builder.Build().RunAsync();
