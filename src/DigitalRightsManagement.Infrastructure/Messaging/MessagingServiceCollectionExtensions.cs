using DigitalRightsManagement.Application.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace DigitalRightsManagement.Infrastructure.Messaging;

internal static class MessagingServiceCollectionExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services) =>
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblyContaining<IUnitOfWork>());
}
