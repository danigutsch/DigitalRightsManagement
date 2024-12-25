using Ardalis.Result.AspNetCore;
using DigitalRightsManagement.Application;
using DigitalRightsManagement.Domain.UserAggregate;

namespace DigitalRightsManagement.Api;

internal static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapGroup("/users")
            .MapPost("/change-role", ChangeRole);
    }

    private static async Task<IResult> ChangeRole(ChangeUserDto dto, ChangeUserRoleCommandHandler handler, CancellationToken ct)
    {
        var command = new ChangeUserRoleCommand(dto.AdminId, dto.TargetId, dto.DesiredRole);
        var result = await handler.Handle(command, ct);

        return result.ToMinimalApiResult();
    }
}

internal sealed record ChangeUserDto(Guid AdminId, Guid TargetId, UserRoles DesiredRole);
