using Microsoft.AspNetCore.Http.HttpResults;

namespace DigitalRightsManagement.Api;

public static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder ProducesDefault(this RouteHandlerBuilder builder) =>
        builder.Produces<Ok>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
}
