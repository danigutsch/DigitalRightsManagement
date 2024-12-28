using Ardalis.Result.AspNetCore;
using DigitalRightsManagement.Application;
using DigitalRightsManagement.Domain.UserAggregate;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DigitalRightsManagement.Api;

internal static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapGroup("/users")
            .WithTags("Users")
            .WithOpenApi()
            .MapPost("/change-role", ChangeRole)
            .WithName("ChangeUserRole")
            .WithSummary("Change the role of a user")
            .WithDescription("Allows an admin to change the role of a target user to a desired role.")
            .Produces<NoContent>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> ChangeRole(ChangeUserDto dto, ChangeUserRoleCommandHandler handler, CancellationToken ct)
    {
        var command = new ChangeUserRoleCommand(dto.AdminId, dto.TargetId, dto.DesiredRole);
        var result = await handler.Handle(command, ct);

        return result.ToMinimalApiResult();
    }
}

public sealed record ChangeUserDto(Guid AdminId, Guid TargetId, UserRoles DesiredRole);
