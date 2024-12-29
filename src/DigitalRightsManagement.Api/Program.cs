using DigitalRightsManagement.Api;
using DigitalRightsManagement.Api.Endpoints;
using DigitalRightsManagement.Application;
using DigitalRightsManagement.Infrastructure;
using DigitalRightsManagement.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder
    .AddApplication()
    .AddInfrastructure();

builder.Services.AddOpenApi();

var app = builder.Build();

// TODÖ: Move to later in pipeline after authentication is added. Also remove corresponding comments in method.
app.MapDefaultEndpoints();

app.UseOpenApi();

app.UseHttpsRedirection();

app.MapUserEndpoints();
app.MapProductEndpoints();

await app.RunAsync();
