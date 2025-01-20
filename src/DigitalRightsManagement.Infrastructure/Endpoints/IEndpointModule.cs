using Microsoft.AspNetCore.Routing;

namespace DigitalRightsManagement.Infrastructure.Endpoints;

/// <summary>
/// Base interface for all endpoint modules.
/// </summary>
internal interface IEndpointModule
{
    IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder builder);
}
