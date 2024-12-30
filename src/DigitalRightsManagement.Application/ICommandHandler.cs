using DigitalRightsManagement.Common.Messaging;
using MediatR;

namespace DigitalRightsManagement.Application;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>;
