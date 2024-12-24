var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddPostgres("database")
    .WithPgWeb();

builder.AddProject<Projects.DigitalRightsManagement_Api>("digitalrightsmanagement-api")
    .WithReference(database)
    .WaitFor(database);

await builder.Build().RunAsync();
