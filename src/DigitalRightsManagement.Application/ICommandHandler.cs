using DigitalRightsManagement.Common.Messaging;

namespace DigitalRightsManagement.Application;

public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<TResponse> Handle(TCommand command, CancellationToken ct);
}
