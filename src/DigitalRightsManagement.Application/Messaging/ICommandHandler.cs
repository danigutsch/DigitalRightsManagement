using Ardalis.Result;
using DigitalRightsManagement.Common.Messaging;
using MediatR;

namespace DigitalRightsManagement.Application.Messaging;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand<Result>;
