using Ardalis.Result;
using DigitalRightsManagement.Application;
using DigitalRightsManagement.Application.Authorization;
using DigitalRightsManagement.Domain;
using DigitalRightsManagement.Domain.UserAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace DigitalRightsManagement.Infrastructure.Messaging.Behaviors;

public sealed class AuthorizationBehavior<TRequest, TResponse>(
    ICurrentUserProvider currentUserProvider,
    ILogger<AuthorizationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResult
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var authorizeAttribute = request.GetType().GetCustomAttribute<AuthorizeAttribute>();

        if (authorizeAttribute is null)
        {
            return await next();
        }

        var userResult = await currentUserProvider.Get(cancellationToken);
        if (!userResult.IsSuccess)
        {
            return (TResponse)(IResult)Errors.User.NotFound();
        }

        var user = userResult.Value;

        if (user.Role < authorizeAttribute.RequiredRole)
        {
            logger.AuthorizationFailed(typeof(TRequest).Name, authorizeAttribute.RequiredRole);
            return (TResponse)(IResult)Errors.User.Unauthorized(authorizeAttribute.RequiredRole);
        }

        return await next();
    }
}

internal static partial class AuthorizationBehaviorLogger
{
    [LoggerMessage(LogLevel.Warning, "Authorization failed for request {Request}. User must be at least {RequiredRole}")]
    public static partial void AuthorizationFailed(this ILogger logger, string request, UserRoles requiredRole);
}
