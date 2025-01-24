using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Infrastructure.Persistence;
using MediatR;

namespace DigitalRightsManagement.Infrastructure.Messaging;

internal static class PublisherExtensions
{
    public static async Task PublishDomainEvents(this IPublisher publisher, ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        var domainEvents = dbContext.ChangeTracker.Entries<AggregateRoot>()
            .SelectMany(e => e.Entity.PopDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent, cancellationToken);
        }
    }
}
