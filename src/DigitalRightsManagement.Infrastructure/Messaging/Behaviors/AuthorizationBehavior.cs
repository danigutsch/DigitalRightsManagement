using Ardalis.Result;
using DigitalRightsManagement.Application;
using DigitalRightsManagement.Application.Authorization;
using DigitalRightsManagement.Domain;
using DigitalRightsManagement.Domain.UserAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace DigitalRightsManagement.Infrastructure.Messaging.Behaviors;

internal sealed class AuthorizationBehavior<TRequest, TResponse>(
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
            return (TResponse)(IResult)userResult;
        }

        if (authorizeAttribute.RequiredRole is null)
        {
            return await next();
        }

        var user = userResult.Value;
        var requiredRole = authorizeAttribute.RequiredRole.Value;

        if (user.Role > requiredRole)
        {
            logger.AuthorizationFailed(typeof(TRequest).Name, requiredRole);
            return (TResponse)(IResult)Errors.User.Unauthorized(requiredRole);
        }

        return await next();
    }
}

internal static partial class AuthorizationBehaviorLogger
{
    [LoggerMessage(LogLevel.Warning, "Authorization failed for request {Request}. User must be at least {RequiredRole}")]
    public static partial void AuthorizationFailed(this ILogger logger, string request, UserRoles requiredRole);
}
