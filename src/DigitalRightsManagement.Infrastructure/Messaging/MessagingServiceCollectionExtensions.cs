using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Infrastructure.Messaging.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace DigitalRightsManagement.Infrastructure.Messaging;

internal static class MessagingServiceCollectionExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services) =>
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblyContaining<IUnitOfWork>();

            configuration.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });
}
