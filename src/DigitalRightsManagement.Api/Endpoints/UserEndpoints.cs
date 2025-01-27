using Ardalis.Result.AspNetCore;
using DigitalRightsManagement.Application.AgentAggregate;
using DigitalRightsManagement.Domain.AgentAggregate;
using DigitalRightsManagement.Infrastructure.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DigitalRightsManagement.Api.Endpoints;

internal class AgentsEndpoints : EndpointGroupBase
{
    protected override string GroupName => "Agents";
    protected override string BaseRoute => "/agents";

    protected override RouteGroupBuilder ConfigureEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/me", GetCurrentAgentInformation)
            .WithName("Get Current Agent")
            .WithSummary("Get current agent information")
            .WithDescription("Returns the information of the currently authenticated agent.")
            .Produces<AgentDto>();

        group.MapPost("/change-role", ChangeRole)
            .WithName("Change Agent Role")
            .WithSummary("Change the role of another agent")
            .WithDescription("Allows an admin to change the role of a target agent to a desired role.")
            .ProducesDefault();

        group.MapPost("/change-email", ChangeEmail)
            .WithName("Change Email")
            .WithSummary("Change the e-mail of an agent")
            .WithDescription("Allows an agent to change his/her e-mail address.")
            .ProducesDefault();

        return group;
    }

    private static async Task<IResult> GetCurrentAgentInformation([FromServices] IMediator mediator, CancellationToken ct)
    {
        var query = new GetCurrentAgentInformationQuery();
        var result = await mediator.Send(query, ct);

        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> ChangeRole([FromBody] ChangeRoleDto dto, [FromServices] IMediator mediator, CancellationToken ct)
    {
        var command = new ChangeAgentRoleCommand(dto.TargetId, dto.DesiredRole);
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

public sealed record ChangeRoleDto(Guid TargetId, AgentRoles DesiredRole);

public sealed record ChangeEmailDto(string NewEmail);
