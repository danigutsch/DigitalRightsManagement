var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.DigitalRightsManagement_Api>("digitalrightsmanagement-api");

builder.Build().Run();
