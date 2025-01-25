using Ardalis.Result;
using MediatR;

namespace DigitalRightsManagement.Common.Messaging;

#pragma warning disable CA1040
public interface ICommand : IRequest<Result>;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>;
#pragma warning restore CA1040
