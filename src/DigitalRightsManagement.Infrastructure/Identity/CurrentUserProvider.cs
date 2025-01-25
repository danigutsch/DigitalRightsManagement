﻿using Ardalis.Result;
using DigitalRightsManagement.Application;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Domain.UserAggregate;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DigitalRightsManagement.Infrastructure.Identity;

internal sealed class CurrentUserProvider(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository) : ICurrentUserProvider
{
    public async Task<Result<User>> Get(CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);

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
