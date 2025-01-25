using DigitalRightsManagement.Infrastructure.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace DigitalRightsManagement.Infrastructure.Messaging.Behaviors;

internal sealed class TransactionBehavior<TRequest, TResponse>(
    ApplicationDbContext dbContext,
    ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var typeName = typeof(TRequest).Name;

        try
        {
            if (dbContext.Database.CurrentTransaction is not null)
            {
                return await next();
            }

            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            using (logger.BeginScope(new List<KeyValuePair<string, object>> { new("TransactionContext", transaction.TransactionId) }))
            {
                logger.BeginTransaction(transaction.TransactionId, typeName, request);

                var response = await next();

                logger.CommitTransaction(transaction.TransactionId, typeName);

                await transaction.CommitAsync(cancellationToken);

                return response;
            }
        }
        catch (Exception ex)
        {
            Activity.Current?.AddException(ex);

            logger.TransactionError(ex, typeName, request);

            throw;
        }
    }
}

internal static partial class TransactionBehaviorLogger
{
    [LoggerMessage(LogLevel.Information, "Begin transaction {TransactionId} for {CommandName} ({@Command})")]
    public static partial void BeginTransaction(this ILogger logger, Guid transactionId, string commandName, object command);

    [LoggerMessage(LogLevel.Information, "Commit transaction {TransactionId} for {CommandName}")]
    public static partial void CommitTransaction(this ILogger logger, Guid transactionId, string commandName);

    [LoggerMessage(LogLevel.Error, "Error Handling transaction for {CommandName} ({@Command})")]
    public static partial void TransactionError(this ILogger logger, Exception exception, string commandName, object command);
}
