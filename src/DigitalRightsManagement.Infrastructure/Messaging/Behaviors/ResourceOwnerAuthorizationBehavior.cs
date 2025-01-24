using Ardalis.Result;
using DigitalRightsManagement.Application;
using DigitalRightsManagement.Application.Authorization;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Reflection;

namespace DigitalRightsManagement.Infrastructure.Messaging.Behaviors;

internal sealed class ResourceOwnerAuthorizationBehavior<TRequest, TResponse>(
    ICurrentUserProvider currentUserProvider,
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
        var attribute = request.GetType()
            .GetCustomAttributes()
            .FirstOrDefault(a => a.GetType().IsGenericType &&
                               a.GetType().GetGenericTypeDefinition() == typeof(AuthorizeResourceOwnerAttribute<>));

        if (attribute is null)
        {
            return await next();
        }

        var attributeType = attribute.GetType();
        var idPropertyPath = attributeType.GetProperty(nameof(AuthorizeResourceOwnerAttribute<AggregateRoot>.IdPropertyPath))!
            .GetValue(attribute) as string
            ?? throw new InvalidOperationException("IdPropertyPath cannot be null");

        var userResult = await currentUserProvider.Get(cancellationToken);
        if (!userResult.IsSuccess)
        {
            return (TResponse)(IResult)userResult;
        }

        var user = userResult.Value;

        var resourceIds = GetResourceIds(request, idPropertyPath);
        if (resourceIds.Length == 0)
        {
            logger.InvalidResourceId(typeof(TRequest).Name);
            throw new InvalidOperationException($"Invalid resource ID for request {typeof(TRequest).Name}");
        }

        var resourceType = attributeType.GetGenericArguments()[0];

        var isOwner = await resourceRepository.IsResourceOwner(user.Id, resourceType, resourceIds, cancellationToken);

        if (!isOwner)
        {
            logger.UnauthorizedResourceAccess(typeof(TRequest).Name, user.Id);
            return (TResponse)(IResult)Result.Unauthorized();
        }

        return await next();
    }

    private static Guid[] GetResourceIds(TRequest request, string propertyPath)
    {
        var type = request.GetType();
        var property = type.GetProperty(propertyPath)
            ?? throw new InvalidOperationException($"Property {propertyPath} not found on type {type.Name}");

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

    [LoggerMessage(LogLevel.Warning, "User {UserId} attempted unauthorized resource access via {Request}")]
    public static partial void UnauthorizedResourceAccess(this ILogger logger, string request, Guid userId);
}
