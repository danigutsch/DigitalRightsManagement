using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace DigitalRightsManagement.Infrastructure.Endpoints;

/// <summary>
/// Represents a group of related endpoints.
/// </summary>
public abstract class EndpointGroupBase : IEndpointModule
{
    protected abstract string GroupName { get; }
    protected abstract string BaseRoute { get; }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(BaseRoute)
            .WithTags(GroupName);

        ConfigureEndpoints(group);

        return builder;
    }

    protected abstract RouteGroupBuilder ConfigureEndpoints(RouteGroupBuilder group);
}
