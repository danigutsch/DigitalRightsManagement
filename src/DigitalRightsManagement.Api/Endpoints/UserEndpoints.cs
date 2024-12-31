using Ardalis.Result.AspNetCore;
using DigitalRightsManagement.Application.ProductAggregate;
using DigitalRightsManagement.Application.UserAggregate;
using DigitalRightsManagement.Domain.UserAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DigitalRightsManagement.Api.Endpoints;

internal static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/users/{userId}")
            .WithTags("Users");

        group.MapGet("/projects", GetProjects)
            .WithName("GetUserProjects")
            .WithSummary("Get the projects of a user")
            .WithDescription("Allows a user to get the projects he/she is a part of.")
            .Produces<ProductDto[]>();

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

    private static async Task<IResult> GetProjects([FromRoute] Guid userId, [FromServices] IMediator mediator, CancellationToken ct)
    {
        var query = new GetProductsQuery(userId);
        var result = await mediator.Send(query, ct);

        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> ChangeRole([FromRoute] Guid userId, [FromBody] ChangeUserDto dto, [FromServices] IMediator mediator, CancellationToken ct)
    {
        var command = new ChangeUserRoleCommand(userId, dto.TargetId, dto.DesiredRole);
        var result = await mediator.Send(command, ct);

        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> ChangeEmail([FromRoute] Guid userId, [FromBody] ChangeEmailDto dto, [FromServices] IMediator mediator, CancellationToken ct)
    {
        var command = new ChangeEmailCommand(userId, dto.NewEmail);
        var result = await mediator.Send(command, ct);

        return result.ToMinimalApiResult();
    }
}

public sealed record ChangeUserDto(Guid TargetId, UserRoles DesiredRole);

public sealed record ChangeEmailDto(string NewEmail);
