var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddPostgres("database")
    .WithPgWeb();

var dbInitializer = builder.AddProject<Projects.DigitalRightsManagement_MigrationService>("digitalrightsmanagement-migrationservice")
    .WithReference(database)
    .WaitFor(database);

builder.AddProject<Projects.DigitalRightsManagement_Api>("digitalrightsmanagement-api")
    .WithReference(database)
    .WaitFor(database)
    .WithReference(dbInitializer)
    .WaitForCompletion(dbInitializer);

await builder.Build().RunAsync();
