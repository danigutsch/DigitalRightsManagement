using Ardalis.Result.AspNetCore;
using DigitalRightsManagement.Application;
using DigitalRightsManagement.Domain.UserAggregate;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DigitalRightsManagement.Api;

internal static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/users")
            .WithTags("Users");

        group.MapPost("/change-role", ChangeRole)
            .WithName("ChangeUserRole")
            .WithSummary("Change the role of another user")
            .WithDescription("Allows an admin to change the role of a target user to a desired role.")
            .Produces<Ok>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/change-email", ChangeEmail)
            .WithName("ChangeEmail")
            .WithSummary("Change the role of a user")
            .WithDescription("Allows an user to change his/her e-mail address.")
            .Produces<Ok>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> ChangeRole(ChangeUserDto dto, ChangeUserRoleCommandHandler handler, CancellationToken ct)
    {
        var command = new ChangeUserRoleCommand(dto.AdminId, dto.TargetId, dto.DesiredRole);
        var result = await handler.Handle(command, ct);

        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> ChangeEmail(ChangeEmailDto dto, ChangeEmailCommandHandler handler, CancellationToken ct)
    {
        var command = new ChangeEmailCommand(dto.UserId, dto.NewEmail);
        var result = await handler.Handle(command, ct);

        return result.ToMinimalApiResult();
    }
}

public sealed record ChangeUserDto(Guid AdminId, Guid TargetId, UserRoles DesiredRole);

public sealed record ChangeEmailDto(Guid UserId, string NewEmail);
