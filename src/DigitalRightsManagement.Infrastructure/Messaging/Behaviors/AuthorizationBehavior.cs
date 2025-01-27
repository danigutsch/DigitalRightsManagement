using Ardalis.Result;
using DigitalRightsManagement.Application;
using DigitalRightsManagement.Application.Authorization;
using DigitalRightsManagement.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;
using DigitalRightsManagement.Domain.AgentAggregate;

namespace DigitalRightsManagement.Infrastructure.Messaging.Behaviors;

internal sealed class AuthorizationBehavior<TRequest, TResponse>(
    ICurrentAgentProvider currentAgentProvider,
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
            return await next()
                .ConfigureAwait(false);
        }

        var agentResult = await currentAgentProvider.Get(cancellationToken);
        if (!agentResult.IsSuccess)
        {
            return (TResponse)(IResult)agentResult.Map();
        }

        if (authorizeAttribute.RequiredRole is null)
        {
            return await next()
                .ConfigureAwait(false);
        }

        var agent = agentResult.Value;
        var requiredRole = authorizeAttribute.RequiredRole.Value;

        if (agent.Role > requiredRole)
        {
            logger.AuthorizationFailed(typeof(TRequest).Name, requiredRole);
            return (TResponse)(IResult)Errors.Agents.Unauthorized(requiredRole);
        }

        return await next()
            .ConfigureAwait(false);
    }
}

internal static partial class AuthorizationBehaviorLogger
{
    [LoggerMessage(LogLevel.Warning, "Authorization failed for request {Request}. Agent must be at least {RequiredRole}")]
    public static partial void AuthorizationFailed(this ILogger logger, string request, AgentRoles requiredRole);
}
