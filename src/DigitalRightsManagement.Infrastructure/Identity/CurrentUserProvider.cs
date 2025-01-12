using Ardalis.Result;
using CommunityToolkit.Diagnostics;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Domain.UserAggregate;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DigitalRightsManagement.Infrastructure.Identity;

internal sealed class CurrentUserProvider(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository) : ICurrentUserProvider
{
    public async Task<Result<User>> Get(CancellationToken ct)
    {
        Guard.IsNotNull(httpContextAccessor.HttpContext);

        var userIdString = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdString))
        {
            return Result.Invalid();
        }

        if (!Guid.TryParse(userIdString, out var userId))
        {
            return Result.Invalid();
        }

        var user = await userRepository.GetById(userId, ct);

        return user;
    }
}
