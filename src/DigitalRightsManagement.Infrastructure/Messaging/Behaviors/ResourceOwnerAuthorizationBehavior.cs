using Ardalis.Result;
using DigitalRightsManagement.Application;
using DigitalRightsManagement.Application.Authorization;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Infrastructure.Persistence.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Reflection;

namespace DigitalRightsManagement.Infrastructure.Messaging.Behaviors;

internal sealed class ResourceOwnerAuthorizationBehavior<TRequest, TResponse>(
    ICurrentUserProvider currentUserProvider,
    ApplicationDbContext dbContext,
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
        var resourceType = attributeType.GetGenericArguments()[0];
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

        var entityType = dbContext.Model.FindEntityType(resourceType)
            ?? throw new InvalidOperationException($"Type {resourceType.Name} is not an entity type");

        _ = entityType.FindProperty("UserId")
            ?? throw new InvalidOperationException($"Entity type {entityType.Name} does not have a UserId property");

        var table = dbContext.GetType().GetProperty(entityType.ClrType.Name + "s")?.GetValue(dbContext)
            ?? throw new InvalidOperationException($"DbSet for {entityType.Name} not found in DbContext");

        var query = (table as IQueryable<object>)
            ?? throw new InvalidOperationException($"Could not get IQueryable for {entityType.Name}");

        var unauthorized = await query
            .Where(e => resourceIds.Contains(EF.Property<Guid>(e, "Id")))
            .AnyAsync(e => EF.Property<Guid>(e, "UserId") != user.Id, cancellationToken);

        if (unauthorized)
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
