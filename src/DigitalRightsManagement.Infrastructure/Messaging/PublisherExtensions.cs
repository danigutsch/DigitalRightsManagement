using DigitalRightsManagement.Common.DDD;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DigitalRightsManagement.Infrastructure.Messaging;

internal static class PublisherExtensions
{
    public static async Task PublishDomainEvents(this IPublisher publisher, DbContext dbContext, CancellationToken cancellationToken)
    {
        var domainEvents = dbContext.ChangeTracker.Entries<AggregateRoot>()
            .SelectMany(e => e.Entity.PopDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent, cancellationToken);
        }
    }
}
