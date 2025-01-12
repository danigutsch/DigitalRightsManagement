using DigitalRightsManagement.Api;
using DigitalRightsManagement.Api.Endpoints;
using DigitalRightsManagement.Application;
using DigitalRightsManagement.Infrastructure;
using DigitalRightsManagement.Infrastructure.Authentication;
using DigitalRightsManagement.Infrastructure.Identity;
using DigitalRightsManagement.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder
    .AddApplication()
    .AddInfrastructure()
    .AddIdentityInfrastructure();

builder.Services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
    .AddBasic();

builder.Services.AddAuthorizationBuilder()
    .AddPolicies();

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi(options => options.AddBasic());

var app = builder.Build();

// TODO: Move to later in pipeline after authentication is added. Also remove corresponding comments in method.
app.MapDefaultEndpoints();

app.UseOpenApi();

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapUserEndpoints();
app.MapProductEndpoints();

await app.RunAsync();
