using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Infrastructure.Persistence;
using MediatR;

namespace DigitalRightsManagement.Infrastructure.Messaging;

internal static class PublisherExtensions
{
    public static async Task PublishDomainEvents(this IPublisher publisher, ManagementDbContext dbContext, CancellationToken cancellationToken)
    {
        DomainEvent[] domainEvents = [.. dbContext.ChangeTracker.Entries<IAggregateRoot>()
            .SelectMany(e => e.Entity.PopDomainEvents())];

        foreach (var domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent, cancellationToken);
        }
    }
}
