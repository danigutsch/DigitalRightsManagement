using Ardalis.Result.AspNetCore;
using DigitalRightsManagement.Application.UserAggregate;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DigitalRightsManagement.Api.Endpoints;

internal static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/users/")
            .WithTags("Users");

        group.MapPost("/change-role", ChangeRole)
            .WithName("ChangeUserRole")
            .WithSummary("Change the role of another user")
            .WithDescription("Allows an admin to change the role of a target user to a desired role.")
            .ProducesDefault()
            .RequireAuthorization(Policies.IsAdmin);

        group.MapPost("/change-email", ChangeEmail)
            .WithName("ChangeEmail")
            .WithSummary("Change the role of a user")
            .WithDescription("Allows an user to change his/her e-mail address.")
            .ProducesDefault()
            .RequireAuthorization();
    }

    private static async Task<IResult> ChangeRole([FromBody] ChangeUserDto dto, [FromServices] IMediator mediator, CancellationToken ct)
    {
        var command = new ChangeUserRoleCommand(dto.TargetId, dto.DesiredRole);
        var result = await mediator.Send(command, ct);

        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> ChangeEmail([FromBody] ChangeEmailDto dto, [FromServices] IMediator mediator, CancellationToken ct)
    {
        var command = new ChangeEmailCommand(dto.NewEmail);
        var result = await mediator.Send(command, ct);

        return result.ToMinimalApiResult();
    }
}

public sealed record ChangeUserDto(Guid TargetId, UserRoles DesiredRole);

public sealed record ChangeEmailDto(string NewEmail);
