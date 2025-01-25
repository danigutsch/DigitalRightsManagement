using Ardalis.Result.AspNetCore;
using DigitalRightsManagement.Application.UserAggregate;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.Infrastructure.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DigitalRightsManagement.Api.Endpoints;

internal class UserEndpoints : EndpointGroupBase
{
    protected override string GroupName => "Users";
    protected override string BaseRoute => "/users";

    protected override RouteGroupBuilder ConfigureEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/me", GetCurrentUserInformation)
            .WithName("GetCurrentUser")
            .WithSummary("Get current user information")
            .WithDescription("Returns the information of the currently authenticated user.")
            .Produces<UserDto>();

        group.MapPost("/change-role", ChangeRole)
            .WithName("ChangeUserRole")
            .WithSummary("Change the role of another user")
            .WithDescription("Allows an admin to change the role of a target user to a desired role.")
            .ProducesDefault();

        group.MapPost("/change-email", ChangeEmail)
            .WithName("ChangeEmail")
            .WithSummary("Change the e-mail of an user")
            .WithDescription("Allows an user to change his/her e-mail address.")
            .ProducesDefault();

        return group;
    }

    private static async Task<IResult> GetCurrentUserInformation([FromServices] IMediator mediator, CancellationToken ct)
    {
        var query = new GetCurrentUserInformationQuery();
        var result = await mediator.Send(query, ct);

        return result.ToMinimalApiResult();
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
