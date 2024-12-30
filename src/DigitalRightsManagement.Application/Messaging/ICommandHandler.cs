using DigitalRightsManagement.Common.Messaging;
using MediatR;

namespace DigitalRightsManagement.Application.Messaging;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>;
