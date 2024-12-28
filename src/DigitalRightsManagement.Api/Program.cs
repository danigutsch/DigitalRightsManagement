using DigitalRightsManagement.Api;
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

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapUserEndpoints();

await app.RunAsync();
