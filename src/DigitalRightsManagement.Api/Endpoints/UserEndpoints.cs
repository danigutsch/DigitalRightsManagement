using Ardalis.Result.AspNetCore;
using DigitalRightsManagement.Application.UserAggregate;
using DigitalRightsManagement.Domain.UserAggregate;
using MediatR;

namespace DigitalRightsManagement.Api.Endpoints;

internal static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/users/{userId}")
            .WithTags("Users");

        group.MapPost("/change-role", ChangeRole)
            .WithName("ChangeUserRole")
            .WithSummary("Change the role of another user")
            .WithDescription("Allows an admin to change the role of a target user to a desired role.")
            .ProducesDefault();

        group.MapPost("/change-email", ChangeEmail)
            .WithName("ChangeEmail")
            .WithSummary("Change the role of a user")
            .WithDescription("Allows an user to change his/her e-mail address.")
            .ProducesDefault();
    }

    private static async Task<IResult> ChangeRole(Guid userId, ChangeUserDto dto, IMediator mediator, CancellationToken ct)
    {
        var command = new ChangeUserRoleCommand(userId, dto.TargetId, dto.DesiredRole);
        var result = await mediator.Send(command, ct);

        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> ChangeEmail(Guid userId, ChangeEmailDto dto, IMediator mediator, CancellationToken ct)
    {
        var command = new ChangeEmailCommand(userId, dto.NewEmail);
        var result = await mediator.Send(command, ct);

        return result.ToMinimalApiResult();
    }
}

public sealed record ChangeUserDto(Guid TargetId, UserRoles DesiredRole);

public sealed record ChangeEmailDto(string NewEmail);
