using DigitalRightsManagement.Api;
using DigitalRightsManagement.Infrastructure;
using DigitalRightsManagement.Infrastructure.Authentication;
using DigitalRightsManagement.Infrastructure.Endpoints;
using DigitalRightsManagement.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder
    .AddInfrastructure()
    .AddIdentityInfrastructure();

builder.Services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
    .AddBasic();

builder.Services.AddAuthorizationBuilder();

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

app.MapEndpointModules(typeof(Program).Assembly);

await app.RunAsync();
