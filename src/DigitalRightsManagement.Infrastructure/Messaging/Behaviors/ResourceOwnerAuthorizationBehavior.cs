using Ardalis.Result;
using DigitalRightsManagement.Application;
using DigitalRightsManagement.Application.Authorization;
using DigitalRightsManagement.Application.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Reflection;

namespace DigitalRightsManagement.Infrastructure.Messaging.Behaviors;

internal sealed class ResourceOwnerAuthorizationBehavior<TRequest, TResponse>(
    ICurrentAgentProvider currentAgentProvider,
    IResourceRepository resourceRepository,
    ILogger<ResourceOwnerAuthorizationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResult
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var attribute = GetAuthorizeResourceOwnerAttribute(request);
        if (attribute is null)
        {
            return await next()
                .ConfigureAwait(false);
        }

        var agentResult = await currentAgentProvider.Get(cancellationToken);
        if (!agentResult.IsSuccess)
        {
            return (TResponse)(IResult)agentResult.Map();
        }

        var agent = agentResult.Value;
        var resourceIds = GetResourceIds(request, attribute.ResourceIdPropertyPath);
        if (resourceIds.Length == 0)
        {
            logger.InvalidResourceId(typeof(TRequest).Name);
            throw new InvalidOperationException($"Invalid resource ID for request {typeof(TRequest).Name}");
        }

        var resourceType = attribute.GetType().GetGenericArguments()[0];
        var isOwner = await resourceRepository.IsResourceOwner(agent.Id, resourceType, resourceIds, cancellationToken);

        if (!isOwner)
        {
            logger.UnauthorizedResourceAccess(typeof(TRequest).Name, agent.Id);
            return (TResponse)(IResult)Result.Unauthorized();
        }

        return await next()
            .ConfigureAwait(false);
    }

    private static AuthorizeResourceOwnerAttribute? GetAuthorizeResourceOwnerAttribute(TRequest request)
    {
        return request.GetType()
            .GetCustomAttributes()
            .Where(a => a.GetType().IsGenericType &&
                        a.GetType().GetGenericTypeDefinition() == typeof(AuthorizeResourceOwnerAttribute<>))
            .Cast<AuthorizeResourceOwnerAttribute>()
            .FirstOrDefault();
    }

    private static Guid[] GetResourceIds(TRequest request, string propertyPath)
    {
        var property = request.GetType().GetProperty(propertyPath)
            ?? throw new InvalidOperationException($"Property {propertyPath} not found on type {request.GetType().Name}");

        var value = property.GetValue(request);
        if (value is null)
        {
            return [];
        }

        return value switch
        {
            Guid id => [id],
            string str when Guid.TryParse(str, out var id) => [id],
            IEnumerable<Guid> ids => [.. ids],
            IEnumerable<string> strings => [..strings.Where(s => Guid.TryParse(s, out _))
                                                .Select(Guid.Parse)],
            IEnumerable enumerable => [..enumerable.Cast<object>()
                                              .Select(ConvertToGuid)
                                              .Where(g => g != Guid.Empty)],
            _ => []
        };
    }

    private static Guid ConvertToGuid(object? value) => value switch
    {
        Guid id => id,
        string str when Guid.TryParse(str, out var id) => id,
        _ => Guid.Empty
    };
}

internal static partial class ResourceOwnerAuthorizationBehaviorLogger
{
    [LoggerMessage(LogLevel.Warning, "Invalid resource ID for request {Request}")]
    public static partial void InvalidResourceId(this ILogger logger, string request);

    [LoggerMessage(LogLevel.Warning, "Agent {AgentId} attempted unauthorized resource access via {Request}")]
    public static partial void UnauthorizedResourceAccess(this ILogger logger, string request, Guid agentId);
}
