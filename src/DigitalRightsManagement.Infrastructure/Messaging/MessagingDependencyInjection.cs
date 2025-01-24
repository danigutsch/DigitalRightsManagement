using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Infrastructure.Messaging.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace DigitalRightsManagement.Infrastructure.Messaging;

internal static class MessagingDependencyInjection
{
    public static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        return services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblyContaining<IUnitOfWork>();

            configuration
                .AddOpenBehavior(typeof(TransactionBehavior<,>))
                .AddOpenBehavior(typeof(AuthorizationBehavior<,>))
                .AddOpenBehavior(typeof(ResourceOwnerAuthorizationBehavior<,>));
        });
    }
}
